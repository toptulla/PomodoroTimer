namespace PomodoroTimer.Api.Persistence
{
    public class DbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
