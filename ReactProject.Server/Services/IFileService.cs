using ReactProject.Server.DTO;
using System.Threading.Tasks;

namespace ReactProject.Server.Services
{
    public interface IFileService
    {
    
        FileStream GetFile(string login, string fileName);
        Task<string> SaveUserFileAsync(IFormFile file, string login);
        List<UserFileDto> GetUserFiles(string login);
    }
}

