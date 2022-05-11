namespace PomodoroTimer
{
    public class Section
    {
        public Section(TimeSpan time, SectionType type)
        {
            Time = time;
            Type = type;
            State = SectionState.Inactive;
        }

        public TimeSpan Time { get; }
        public SectionType Type { get; }
        public SectionState State { get; set; }
    }
}
