using Microsoft.EntityFrameworkCore;
using ReactProject.Server.Entities;
using ReactProject.Server.Model;

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
        

        public async Task SaveFileMetadataAsync(UserFile userFile)
        {
            if (userFile == null)
                throw new ArgumentNullException(nameof(userFile));
            await _dbContext.UserFiles.AddAsync(userFile);
            await _dbContext.SaveChangesAsync();
        }


    }
}
