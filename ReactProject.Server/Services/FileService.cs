using ReactProject.Server.DTO;
using ReactProject.Server.Entities;
using ReactProject.Server.Repositories;
using ReactProject.Server.Services;

public class FileService : IFileService
{
    private readonly string _storagePath;
    private readonly IWebHostEnvironment _env;
    private readonly IUserStorageService _userStorageService;
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;
    public FileService(IWebHostEnvironment env,IConfiguration configuration, IUserStorageService userStorageService, IUserRepository userRepository, IFileRepository fileRepository)
    { 
        
        _env = env;
        _userStorageService = userStorageService;
        _storagePath = _userStorageService.GetStoragePath();
        _userRepository = userRepository;
        _fileRepository = fileRepository;

    }

    // Zapisanie pliku do systemu
    public async Task<string> SaveUserFileAsync(IFormFile file, string login)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Plik jest pusty.");

        var userFolderPath = Path.Combine(_storagePath, login);

        if (!Directory.Exists(userFolderPath))
        {
            await _userStorageService.CreateUserDirectoryAsync(login);
        }

        var filePath = Path.Combine(userFolderPath, file.FileName);

        await _fileRepository.SaveFileToDiskAsync(file, filePath);

        var user = await _userRepository.GetByLoginAsync(login);
        if (user == null)
            throw new InvalidOperationException("Nie znaleziono użytkownika.");

        var userFile = new UserFile
        {
            UserId = user.Id,
            FileName = file.FileName,
            FileType = Path.GetExtension(file.FileName).TrimStart('.'),
            FileSize = file.Length,
            UploadDate = DateTime.UtcNow
        };

        await _fileRepository.SaveFileMetadataAsync(userFile);

        return filePath;
    }

    
    public FileStream GetFile(string login, string fileName)
    {
        var userFolderPath = Path.Combine(_storagePath, login);
        var filePath = Path.Combine(userFolderPath, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Plik nie został znaleziony.");

        return new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    public List<UserFileDto> GetUserFiles(string login)
    {
        var userFolderPath = Path.Combine(_storagePath, login);

        if (!Directory.Exists(userFolderPath))
            return new List<UserFileDto>();

        return Directory.GetFiles(userFolderPath)
            .Select(path => new UserFileDto
            {
                FileName = Path.GetFileName(path),
                Url = $"{Path.GetFileName(path)}"
            })
            .ToList();
    }


}
