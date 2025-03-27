namespace Application.DTOs.User
{
    public class UserResponseDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required bool IsEmailVerified { get; set; }
    }
}
