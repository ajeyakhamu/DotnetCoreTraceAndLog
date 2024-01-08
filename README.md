# DotnetCoreTraceAndLog

## Otel Example

dotnet run --Urls http://0.0.0.0:5140

### Docker container required for Telemetry

[https://github.com/ajeyakhamu/Docker/tree/main/Telemetry](https://github.com/ajeyakhamu/Docker/tree/main/Telemetry)

### Notes:

- For testing purposes the ports are forwarded in some cases
- Grafana products used for GUI

## Jaeger Example

dotnet run --Urls http://0.0.0.0:5081

### Docker container required for Telemetry

[https://github.com/ajeyakhamu/Docker/tree/main/Jaeger](https://github.com/ajeyakhamu/Docker/tree/main/Jaeger)

### Notes:

- The code base is same as the Otel version. This version just has tracing.
- Jaeger all in one image used to view tracing
