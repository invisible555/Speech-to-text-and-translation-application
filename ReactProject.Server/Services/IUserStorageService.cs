namespace ReactProject.Server.Services
{
    public interface IUserStorageService
    {
        string GetStoragePath();
        Task CreateUserDirectoryAsync(string username);
    }
}
