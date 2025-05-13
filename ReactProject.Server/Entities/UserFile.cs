using ReactProject.Server.Model;

namespace ReactProject.Server.Entities
{
    public class UserFile
    {
        public int Id { get; set; }

        // RELACJA z użytkownikiem (wiele plików należy do jednego użytkownika)
        public int UserId { get; set; }
        public User User { get; set; } = null!;


        public string FileName { get; set; } = null!;   // Np. "plik1.wav"
        public string FileType { get; set; } = null!;   // Np. "wav", "txt"
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
        public string? Description { get; set; }
        public string? Category { get; set; }
    }
}
