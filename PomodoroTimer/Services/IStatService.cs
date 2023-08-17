using PomodoroTimer.Common;

namespace PomodoroTimer.Services
{
    public interface IStatService
    {
        Task Add(ActionType actionType, SectionType sectionType);
    }
}
