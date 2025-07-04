﻿namespace ReactProject.Server.Services
{
    public class UserStorageService : IUserStorageService
    {
        private readonly string _storagePath;

        public UserStorageService(IConfiguration configuration, IWebHostEnvironment env)
        {
            var relativePath = configuration["Storage:StoragePath"] ?? "App_Data/UserFiles";
            _storagePath = Path.Combine(env.ContentRootPath, relativePath);
        }


        public Task CreateUserDirectoryAsync(string login)
        {
            var path = Path.Combine(_storagePath, login);

            Directory.CreateDirectory(path+ "transcription");
            Directory.CreateDirectory(path + "files");

            return Task.CompletedTask;
        }
        public string GetStoragePath() => _storagePath;

    }
}
