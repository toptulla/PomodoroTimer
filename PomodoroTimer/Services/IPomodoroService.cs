namespace PomodoroTimer.Services
{
    public interface IPomodoroService
    {
        TimeSpan Elapsed { get; }
        IEnumerable<Section> Sections { get; }

        event Func<Task>? EndInterval;
        event Func<Task>? Progress;

        Task Pause();
        Task Reset();
        Task Skip();
        Task Start();
    }
}