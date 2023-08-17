using System.ComponentModel.DataAnnotations;

namespace PomodoroTimer.Common.Account
{
    public class LogoutModel
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
