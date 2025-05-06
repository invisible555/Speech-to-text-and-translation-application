using ReactProject.Server.Model;

namespace ReactProject.Server.DTO
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public Users? User { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
