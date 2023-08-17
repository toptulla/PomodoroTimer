using System.ComponentModel.DataAnnotations;

namespace PomodoroTimer.Api.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; } = null!;
        public PomodoroUser User { get; set; } = null!;
    }
}
