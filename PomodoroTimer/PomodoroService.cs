using Timer = System.Timers.Timer;

namespace PomodoroTimer
{
    public class PomodoroService : IDisposable
    {
        private readonly Timer timer = new(1000);
        private TimeSpan elapsed = TimeSpan.Zero;
        private readonly Section[] sections;
        private int sectionIndex;

        public PomodoroService(Section[] sections)
        {
            this.sections = sections;
            timer.Elapsed += OnTimerElapsed;
        }

        public int SectionsCount => sections.Length;

        public int CurrentSection => sectionIndex;

        public TimeSpan Elapsed => sections[sectionIndex].Time - elapsed;

        public IEnumerable<Section> Sections => sections;

        public event Func<Task>? Progress;

        public event Func<Task>? EndInterval;

        public void Start()
        {
            timer.Start();

            sections[sectionIndex].State = SectionState.Active;
        }

        public void Skip()
        {
            timer.Stop();

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

        public void Pause()
        {
            timer.Stop();
        }

        public void Reset()
        {
            timer.Stop();

            foreach (var section in sections)
                section.State = SectionState.Inactive;

            sectionIndex = 0;
            elapsed = TimeSpan.Zero;
        }

        private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            elapsed += TimeSpan.FromSeconds(1);

            Progress?.Invoke();

            if (elapsed == sections[sectionIndex].Time)
            {
                Skip();

                EndInterval?.Invoke();
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
