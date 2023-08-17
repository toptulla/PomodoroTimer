using System.ComponentModel.DataAnnotations;

namespace PomodoroTimer.Common.Account
{
    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
