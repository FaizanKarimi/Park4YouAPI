using System;

namespace Components.Services.Solvision.Models
{
    public class LoginResponse
    {
        public string Status { get; set; }
        public string Username { get; set; }
        public string Hash { get; set; }
        public DateTime Expire { get; set; }
    }
}