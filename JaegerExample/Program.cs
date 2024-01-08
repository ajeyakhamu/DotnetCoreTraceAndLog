using System.Reflection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using JaegerExample.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry Resources with the application name
var otel = builder.Services.AddOpenTelemetry();
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

otel.WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();
        t.AddSource(OtelInstrumentation.ActivitySourceName);

        t.AddOtlpExporter(e =>
        {
            e.Endpoint = new Uri("http://localhost:4317");
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

