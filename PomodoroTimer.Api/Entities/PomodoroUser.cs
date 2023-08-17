using Microsoft.AspNetCore.Identity;

namespace PomodoroTimer.Api.Entities
{
    public class PomodoroUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
    }
}
