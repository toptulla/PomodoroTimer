using PomodoroTimer.Common;

namespace PomodoroTimer.Api.Entities
{
    public class StatItem
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public ActionType ActionType { get; set; }
        public SectionType SectionType { get; set; }
        public PomodoroUser User { get; set; } = null!;
    }
}
