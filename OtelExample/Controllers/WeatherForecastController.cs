using Microsoft.AspNetCore.Mvc;
using OtelExample.ExtensionMethods;
using OtelExample.Models;

namespace OtelExample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, OtelInstrumentation instrumentation) : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        using var activity = instrumentation.ActivitySource.StartActivity("calculate forecast");
        activity?.SetTag("app.weather.TemperatureC", Random.Shared.Next(-20, 55));
        activity?.SetTag("app.weather.Summary", Summaries[Random.Shared.Next(Summaries.Length)]);

        var forcasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        instrumentation.FreezingDaysCounter.Add(forcasts.Count(f => f.TemperatureC < 0));

        using (var childActivity = instrumentation.ActivitySource.StartActivity("sample logging"))
        {
            logger.GeneratedForcastsCount(forcasts.Length);
        }

        return forcasts;
    }
}

