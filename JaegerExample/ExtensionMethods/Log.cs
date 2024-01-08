namespace JaegerExample.ExtensionMethods;

public static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "WeatherForecasts generated {count}")]
    public static partial void GeneratedForcastsCount(this ILogger logger, int count);
}
