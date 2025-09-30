namespace WebAPI.DTOs
{
    public class RefreshTokenRequestDto
    {
        public string ExpiredAccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
