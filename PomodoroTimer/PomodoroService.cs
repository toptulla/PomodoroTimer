using PomodoroTimer.Common;
using PomodoroTimer.Services;
using Timer = System.Timers.Timer;

namespace PomodoroTimer
{
    public class PomodoroService : IPomodoroService, IDisposable
    {
        private readonly Timer timer = new(1000);
        private TimeSpan elapsed = TimeSpan.Zero;
        private readonly Section[] sections;
        private readonly IStatService statService;
        private int sectionIndex;

        public PomodoroService(IStatService statService)
        {
            sections = new[]
            {
                new Section(TimeSpan.FromMinutes(25), SectionType.Work),
                new Section(TimeSpan.FromMinutes(5), SectionType.Break),
                new Section(TimeSpan.FromMinutes(25), SectionType.Work),
                new Section(TimeSpan.FromMinutes(5), SectionType.Break),
                new Section(TimeSpan.FromMinutes(25), SectionType.Work),
                new Section(TimeSpan.FromMinutes(5), SectionType.Break),
                new Section(TimeSpan.FromMinutes(25), SectionType.Work),
                new Section(TimeSpan.FromMinutes(15), SectionType.LongBreak)
            };
            timer.Elapsed += OnTimerElapsed;
            this.statService = statService;
        }

        public TimeSpan Elapsed => sections[sectionIndex].Time - elapsed;

        public IEnumerable<Section> Sections => sections;

        public event Func<Task>? Progress;

        public event Func<Task>? EndInterval;

        public async Task Start()
        {
            timer.Start();

            sections[sectionIndex].State = SectionState.Active;

            await statService.Add(ActionType.Start, sections[sectionIndex].Type);
        }

        public async Task Pause()
        {
            timer.Stop();

            await statService.Add(ActionType.Pause, sections[sectionIndex].Type);
        }

        public async Task Skip()
        {
            timer.Stop();

            await statService.Add(ActionType.Skip, sections[sectionIndex].Type);

            if (sectionIndex < sections.Length - 1)
            {
                sections[sectionIndex].State = SectionState.Complete;
                sectionIndex++;
            }
            else
            {
                foreach (var section in sections)
                    section.State = SectionState.Inactive;

                sectionIndex = 0;
            }

            elapsed = TimeSpan.Zero;
        }

        public async Task Reset()
        {
            timer.Stop();

            foreach (var section in sections)
                section.State = SectionState.Inactive;

            await statService.Add(ActionType.Reset, sections[sectionIndex].Type);

            sectionIndex = 0;
            elapsed = TimeSpan.Zero;
        }

        private async void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            elapsed += TimeSpan.FromSeconds(1);

            if (Progress is not null)
                await Progress.Invoke();

            if (elapsed == sections[sectionIndex].Time)
            {
                await Skip();

                if (EndInterval is not null)
                    await EndInterval.Invoke();
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
