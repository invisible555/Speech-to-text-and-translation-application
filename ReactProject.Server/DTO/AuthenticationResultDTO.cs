﻿using ReactProject.Server.Model;

namespace ReactProject.Server.DTO
{
    public class AuthenticationResultDTO
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public User? User { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
