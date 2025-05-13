using System.Threading.Tasks;

namespace ReactProject.Server.Services
{
    public interface IFileService
    {
    
        FileStream GetFile(string username, string fileName);
        Task<string> SaveUserFileAsync(IFormFile file, string username);
        List<string?> GetUserFiles(string username);
    }
}
