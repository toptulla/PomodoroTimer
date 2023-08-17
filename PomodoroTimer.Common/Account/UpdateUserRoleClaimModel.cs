using System.ComponentModel.DataAnnotations;

namespace PomodoroTimer.Common.Account
{
    public class UpdateUserRoleClaimModel
    {
        [Required]
        public string Role { get; set; } = null!;
    }
}
