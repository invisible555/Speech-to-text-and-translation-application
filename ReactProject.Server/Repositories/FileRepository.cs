using Microsoft.EntityFrameworkCore;
using ReactProject.Server.Entities;

namespace ReactProject.Server.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _dbContext; 
        public FileRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> SaveFileToDiskAsync(IFormFile file, string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return path;
        }
        public async Task SaveFileToDatabaseAsync(UserFile userFile)
        {
            _dbContext.UserFiles.Add(userFile);
            await _dbContext.SaveChangesAsync();
        }
    }
}
