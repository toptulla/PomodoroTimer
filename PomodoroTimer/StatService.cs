using PomodoroTimer.Common;
using PomodoroTimer.Services;
using System.Text;

namespace PomodoroTimer
{
    public class StatService : IStatService
    {
        private readonly HttpClient client;

        public StatService(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("api");
        }

        public async Task Add(ActionType actionType, SectionType sectionType)
            => await client.PutAsync("stats/add", new StringContent(new { actionType, sectionType }.ToString()!, Encoding.UTF8, "application/json"));
    }
}
