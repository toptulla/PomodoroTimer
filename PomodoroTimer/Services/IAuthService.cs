using PomodoroTimer.Common.Account;

namespace PomodoroTimer.Services
{
    public interface IAuthService
    {
        Task<LoginResult?> Login(LoginModel loginModel);
        Task<LogoutResult?> Logout();
        Task<RegisterResult?> Register(RegisterModel registerModel);
    }
}
