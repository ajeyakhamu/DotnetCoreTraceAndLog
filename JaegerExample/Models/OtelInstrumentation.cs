using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace JaegerExample.Models;

public class OtelInstrumentation
{
    public const string ActivitySourceName = "OtelExampleInstrumentation";
    private readonly Meter meter;
    public ActivitySource ActivitySource { get; }
    public Counter<long> FreezingDaysCounter { get; }

    public OtelInstrumentation()
    {
        string? version = typeof(OtelInstrumentation).Assembly.GetName().Version?.ToString();
        this.ActivitySource = new ActivitySource(ActivitySourceName, version);
        this.meter = new Meter(ActivitySourceName, version);
        this.FreezingDaysCounter = this.meter.CreateCounter<long>("weather.days.freezing", description: "The number of days where the temperature is below freezing");
    }

    public void Dispose()
    {
        this.ActivitySource.Dispose();
        this.meter.Dispose();
    }
}
