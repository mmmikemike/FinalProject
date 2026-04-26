using System.Reflection.Metadata.Ecma335;

namespace PropertyManagement.API.Models
{
    public class User //basic user model for authentication
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Tenant"; //default role
    }
}
