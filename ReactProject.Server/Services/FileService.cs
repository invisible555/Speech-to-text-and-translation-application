using ReactProject.Server.Services;

public class FileService : IFileService
{
    private readonly string _storagePath;
    private readonly IWebHostEnvironment _env;
    public FileService(IWebHostEnvironment env,IConfiguration configuration)
    { 
        _storagePath = configuration["StoragePath"] ?? Path.Combine(env.ContentRootPath, "App_Data", "UserFiles");
        _env = env;
    }

    // Zapisanie pliku do systemu
    public async Task<string> SaveUserFileAsync(IFormFile file, string username)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Plik jest pusty.");

        var userFolderPath = Path.Combine(_storagePath, username);

        // Jeśli folder użytkownika nie istnieje, tworzymy go
        if (!Directory.Exists(userFolderPath))
        {
            Directory.CreateDirectory(userFolderPath);
        }

        var filePath = Path.Combine(userFolderPath, file.FileName);

        // Zapisujemy plik
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filePath;
    }

    // Pobieranie pliku przez użytkownika
    public FileStream GetFile(string username, string fileName)
    {
        var userFolderPath = Path.Combine(_storagePath, username);
        var filePath = Path.Combine(userFolderPath, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Plik nie został znaleziony.");

        return new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    public List<string?> GetUserFiles(string username)
    {
        var userFolderPath = Path.Combine(_storagePath, username);

        if (!Directory.Exists(userFolderPath))
            return new List<string?>(); // Brak folderu = brak plików

        // Pobierz tylko nazwy plików, bez pełnych ścieżek
        var files = Directory.GetFiles(userFolderPath)
                             .Select(Path.GetFileName)
                             .ToList();

        return files;
    }
    public void CreateUserDirectoryAsync(string username)
    {
        var userFolderPath = Path.Combine(_storagePath, username);

        if (!Directory.Exists(userFolderPath))
        {
            Directory.CreateDirectory(userFolderPath);
        }

         // Metoda jest async, żeby była spójna, ale operacja jest synchroniczna
    }

}
