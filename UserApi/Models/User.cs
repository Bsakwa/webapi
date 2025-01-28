namespace UserApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty; // Initialize with default value
        public string Email { get; set; } = string.Empty; // Initialize with default value
        public string PasswordHash { get; set; } = string.Empty; // Initialize with default value
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
