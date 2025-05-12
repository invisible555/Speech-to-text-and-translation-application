namespace ReactProject.Server.Services
{
    public interface IUserStorageService
    {
        Task CreateUserDirectoryAsync(string username);
    }
}
