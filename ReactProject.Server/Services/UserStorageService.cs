namespace ReactProject.Server.Services
{
    public class UserStorageService : IUserStorageService
    {
        private readonly string _storagePath;

        public UserStorageService(IConfiguration configuration, IWebHostEnvironment env)
        {
            var relativePath = configuration["Storage:StoragePath"];
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentException("Brak StoragePath w konfiguracji.");
            }

          
            _storagePath = Path.Combine(env.ContentRootPath, relativePath);
        }

        public Task CreateUserDirectoryAsync(string username)
        {
            var path = Path.Combine(_storagePath, username);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Task.CompletedTask;
        }
    }
}
