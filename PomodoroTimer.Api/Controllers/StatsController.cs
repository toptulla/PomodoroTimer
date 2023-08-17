using Microsoft.AspNetCore.Mvc;
using PomodoroTimer.Api.Persistence;
using PomodoroTimer.Common;

namespace PomodoroTimer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly PomodoroTimerDbContext dbContext;

        public StatsController(PomodoroTimerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPut("add")]
        public async Task<IActionResult> Start(ActionType actionType, SectionType sectionType, string userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user is not null)
            {
                await dbContext.StatItems.AddAsync(new Entities.StatItem
                {
                    Date = DateTime.Now,
                    ActionType = actionType,
                    SectionType = sectionType,
                    User = user
                });
                await dbContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
