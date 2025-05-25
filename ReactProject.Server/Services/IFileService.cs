using ReactProject.Server.DTO;
using System.Threading.Tasks;

namespace ReactProject.Server.Services
{
    public interface IFileService
    {
        
        Task<string> SaveUserFileAsync(IFormFile file, string login,string language);
        FileWithStreamDTO GetAudioFile(string login, string relativeFilePath);
        List<UserFileDto> GetUserFiles(string login);
        Task GenerateTranscriptionAsync(string sourceLang, string targetLang, string originalFilepath,string user);


        Task ConvertFileToWav(string filepath,string login);
   
        Task<string?> GetTranscriptionAsync(string fileName,string login);
    }
}

