// GraphQL/Types/Inputs/UpdateUserInput.cs
namespace UserApi.GraphQL.Types.Inputs
{
    public class UpdateUserInput
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
