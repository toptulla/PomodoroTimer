namespace PomodoroTimer.Api.Persistence
{
    public class PomodoroTimerDbContextOptions
    {
        public string ConnectionString { get; set; } = null!;
        public bool HasConsoleLogging { get; set; }
    }
}
