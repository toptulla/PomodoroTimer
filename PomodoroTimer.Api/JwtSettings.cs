namespace PomodoroTimer.Api
{
    public class JwtSettings
    {
        public string Audience { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string AccessTokenSecurityKey { get; set; } = null!;
        public double AccessTokenExpiryInMinutes { get; set; }
        public string RefreshTokenSecurityKey { get; set; } = null!;
        public double RefreshTokenExpiryInMinutes { get; set; }
    }
}
