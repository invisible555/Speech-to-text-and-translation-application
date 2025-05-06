namespace ReactProject.Server.Entities
{
    public class UserAccessTokens
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
