using Microsoft.IdentityModel.Tokens;
using ReactProject.Server.DTO;
using ReactProject.Server.Entities;
using ReactProject.Server.Model;
using ReactProject.Server.Repositories;
using ReactProject.Server.Services;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

public class FileService : IFileService
{
    private readonly string _storagePath;
    private readonly IWebHostEnvironment _env;
    private readonly IUserStorageService _userStorageService;
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;
    private readonly HttpClient _client;
    public FileService(IWebHostEnvironment env, IConfiguration configuration, IUserStorageService userStorageService, IUserRepository userRepository, IFileRepository fileRepository, IHttpClientFactory client)
    {

        _env = env;
        _userStorageService = userStorageService;
        _storagePath = _userStorageService.GetStoragePath();
        _userRepository = userRepository;
        _fileRepository = fileRepository;
        _client = client.CreateClient("TranscriptionApi");


    }


    public async Task<string> SaveUserFileAsync(IFormFile file, string login, string language)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Plik jest pusty.");

        var userFolderPath = Path.Combine(_storagePath, login, "files");

        if (!Directory.Exists(userFolderPath))
            await _userStorageService.CreateUserDirectoryAsync(login);

        
        var originalFilePath = Path.Combine(userFolderPath, file.FileName);
        await _fileRepository.SaveFileToDiskAsync(file, originalFilePath);

 
        var relativeFileName = Path.GetFileName(file.FileName);
        try
        {
         
            await ConvertFileToWav(relativeFileName, login);
        }
        catch
        {
            return "";
        }

  
        var wavFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".wav";
        var wavFilePath = Path.Combine(userFolderPath, wavFileName);

    
        if (!File.Exists(wavFilePath))
            throw new Exception("Plik WAV nie został utworzony.");

        var user = await _userRepository.GetByLoginAsync(login);
        if (user == null)
            throw new InvalidOperationException("Nie znaleziono użytkownika.");

        var fileInfo = new FileInfo(wavFilePath);

        var userFile = new UserFile
        {
            UserId = user.Id,
            FileName = wavFileName,
            FileType = "wav",
            FileSize = fileInfo.Length,
            UploadDate = DateTime.UtcNow
        };

        await _fileRepository.SaveFileMetadataAsync(userFile);

        return wavFilePath;
    }


    public FileWithStreamDTO GetAudioFile(string login, string relativeFilePath)
{
    var userFolderPath = Path.Combine(_storagePath, login);
    var filePath = Path.Combine(userFolderPath,"files", relativeFilePath);

    if (!File.Exists(filePath))
        throw new FileNotFoundException("Plik nie został znaleziony.");

    string extension = Path.GetExtension(filePath).ToLowerInvariant();
    string[] allowedAudioExtensions = { ".mp3", ".wav", ".ogg", ".flac", ".aac", ".m4a" };

    if (!allowedAudioExtensions.Contains(extension))
        throw new InvalidOperationException("Dozwolone są tylko pliki audio.");

    return new FileWithStreamDTO
    {
        Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read),
        FileName = Path.GetFileName(filePath),
        ContentType = GetContentType(filePath),
    };
}
    public List<UserFileDto> GetUserFiles(string login)
    {
        var userFilesPath = Path.Combine(_storagePath, login, "files");

        if (!Directory.Exists(userFilesPath))
            return new List<UserFileDto>();

        return Directory.GetFiles(userFilesPath)
            .Select(path => new UserFileDto
            {
                FileName = Path.GetFileName(path),
                Url = $"File/download/files/{Path.GetFileName(path)}"
            })
            .ToList();
    }

    public async Task GenerateTranscriptionAsync(string sourceLang, string fileName, string user)
    {
        var fullPath = Path.Combine(_storagePath, user, fileName);
        if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fullPath))
            throw new FileNotFoundException("Plik do transkrypcji nie istnieje.", fullPath);

        var requestObj = new
        {
            source_lang = sourceLang,
            original_filepath = fullPath
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestObj),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("transcribe", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Błąd transkrypcji: {error}");
        }
    }

    public async Task<string> TranslateTextAsync(string text, string sourceLang, string targetLang)
    {
        var requestObj = new
        {
            q = text,
            source = sourceLang,
            target = targetLang,
            format = "text"
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestObj),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("translate", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Błąd tłumaczenia: {error}");
        }

        var resultJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(resultJson);
        return doc.RootElement.GetProperty("translatedText").GetString() ?? "";
    }

    public async Task ConvertFileToWav(string filepath, string login)
    {
        var content = ConvertRequest(login, filepath);

        var response = await _client.PostAsync("convert", content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Błąd konwertowania na wav: {error}");
        }
    }
    private StringContent ConvertRequest(string username, string filename)
    {
        var payload = new
        {
            username = username,
            filename = filename
        };

        return new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );
    }
    public async Task<string?> GetTranscriptionAsync(string fileName,string login)
    {
        var transcriptionFileName = Path.ChangeExtension(fileName, ".txt"); 
        var transcriptionPath = Path.Combine(_storagePath,login, "transcription", transcriptionFileName);

        if (!File.Exists(transcriptionPath))
        {
            return null;
        }

        return await File.ReadAllTextAsync(transcriptionPath);
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".csv" => "text/csv",
            ".pdf" => "application/pdf",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".mp4" => "video/mp4",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".html" => "text/html",
            _ => "application/octet-stream"
        };
    }
}
