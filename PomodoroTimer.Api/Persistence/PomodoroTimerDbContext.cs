using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PomodoroTimer.Api.Entities;

namespace PomodoroTimer.Api.Persistence
{
    public class PomodoroTimerDbContext : IdentityDbContext<PomodoroUser>
    {
        private readonly string connectionString;
        private readonly bool hasConsoleLogging;

        public PomodoroTimerDbContext(PomodoroTimerDbContextOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new PersistenceConfigurationException($"{nameof(options.ConnectionString)} is null or empty");

            connectionString = options.ConnectionString!;
            hasConsoleLogging = options.HasConsoleLogging;
        }

        public DbSet<RefreshToken> BlackListRefreshTokens => Set<RefreshToken>();
        public DbSet<StatItem> StatItems => Set<StatItem>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);

            if (hasConsoleLogging)
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddFilter((category, level) =>
                            category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                        .AddConsole();
                });
                optionsBuilder
                    .UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PomodoroUser>().HasData(
                new PomodoroUser
                {

                });
        }
    }
}
