using System.Reflection;

using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;

using OtelExample.Models;


var builder = WebApplication.CreateBuilder(args);

var otel = builder.Services.AddOpenTelemetry();

// Configure OpenTelemetry Resources with the application name
otel.ConfigureResource(resource =>
    {
        resource.AddService(
                serviceNamespace: "OtelExample",
                serviceName: builder.Environment.ApplicationName,
                serviceVersion: Assembly.GetEntryAssembly()?.GetName().Version?.ToString(),
                serviceInstanceId: Environment.MachineName
        ).AddAttributes(new Dictionary<string, object>
            {
                { "deployment.environment", builder.Environment.EnvironmentName }
            });
    });

otel.WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();
        m.AddRuntimeInstrumentation();
        m.AddMeter("Microsoft.Net.Http");
        //m.AddMeter("Microsoft.Extensions.Diagnostics.HealthChecks");

        m.AddOtlpExporter(e =>
        {
            e.Endpoint = new Uri("http://localhost:4319/v1/metrics");
            e.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    });

otel.WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();
        t.AddSource(OtelInstrumentation.ActivitySourceName);

        t.AddOtlpExporter(e =>
        {
            e.Endpoint = new Uri("http://localhost:4319/v1/traces");
            e.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    });

builder.Logging.AddOpenTelemetry(l =>
    {
        l.IncludeFormattedMessage = true;
        l.IncludeScopes = true;
        l.ParseStateValues = true;

        l.AddOtlpExporter(e =>
        {
            e.Endpoint = new Uri("http://localhost:4319/v1/logs");
            e.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<OtelInstrumentation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

