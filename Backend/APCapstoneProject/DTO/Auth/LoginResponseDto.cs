namespace APCapstoneProject.DTO.Auth
{
    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
    }
}
