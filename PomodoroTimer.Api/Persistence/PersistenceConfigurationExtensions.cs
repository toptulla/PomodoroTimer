namespace PomodoroTimer.Api.Persistence
{
    public static class PersistenceConfigurationExtensions
    {
        public static void AddPersistence(this IServiceCollection services, Action<PomodoroTimerDbContextOptions> optionsSetup)
        {
            var options = new PomodoroTimerDbContextOptions();
            optionsSetup(options);

            services.AddScoped(_ => new PomodoroTimerDbContext(options));
        }
    }
}
