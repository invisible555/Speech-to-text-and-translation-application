using ReactProject.Server.Model;

namespace ReactProject.Server.DTO
{
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User {  get; set; }
    }
}
