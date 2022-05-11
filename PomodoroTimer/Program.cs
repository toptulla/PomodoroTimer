using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PomodoroTimer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<PomodoroService>(sp => new PomodoroService(new[]
{
    new Section(TimeSpan.FromMinutes(25), SectionType.Work),
    new Section(TimeSpan.FromMinutes(5), SectionType.Break),
    new Section(TimeSpan.FromMinutes(25), SectionType.Work),
    new Section(TimeSpan.FromMinutes(5), SectionType.Break),
    new Section(TimeSpan.FromMinutes(25), SectionType.Work),
    new Section(TimeSpan.FromMinutes(5), SectionType.Break),
    new Section(TimeSpan.FromMinutes(25), SectionType.Work),
    new Section(TimeSpan.FromMinutes(15), SectionType.LongBreak)
}));

await builder.Build().RunAsync();
