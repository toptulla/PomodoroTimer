namespace PomodoroTimer.Api.Persistence
{
    public class PersistenceConfigurationException : ApplicationException
    {
        public PersistenceConfigurationException(string message)
            : base(message)
        {

        }
    }
}
