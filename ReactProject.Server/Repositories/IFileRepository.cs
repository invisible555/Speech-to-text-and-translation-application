using ReactProject.Server.Entities;

namespace ReactProject.Server.Repositories
{
    public interface IFileRepository
    {

        Task<string> SaveFileToDiskAsync(IFormFile file, string path);
        Task SaveFileToDatabaseAsync(UserFile userFile);
    }
}
