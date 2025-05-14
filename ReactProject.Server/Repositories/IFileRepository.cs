using ReactProject.Server.Entities;

namespace ReactProject.Server.Repositories
{
    public interface IFileRepository
    {

        Task<string> SaveFileToDiskAsync(IFormFile file, string filePath);
        Task SaveFileMetadataAsync(UserFile userFile);
    }

}
