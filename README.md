# Microsoft.ApplicationInsights.ServiceContext
The package provides extension for all [ApplicationInsights .NET SDK flavors](https://github.com/Microsoft/ApplicationInsights-dotnet) that sets application level properties (name, version and arbitrary custom properties) via environment variables.

## Usage

### ASP.NET Core Web Application

Register `ServiceContextTelemetryIntitializer` singleton as implementation of `ITelemetryInitializer` with ASP.NET Core dependency injection.

```C#
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceContext;

...

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ITelemetryInitializer, ServiceContextTelemetryIntitializer>();
    }
```

See ASP.NET Core [example](samples\AspNetCore\Startup.cs#L22)

### Console Application

Add `ServiceContextTelemetryIntitializer` to the `TelemetryConfigration`. You may use default `Active` configuration, or instantiate another one and use it when configuring `TelemetryClient`

```C#
    TelemetryConfiguration.Active.TelemetryInitializers.Add(new ServiceContextTelemetryIntitializer());
    TelemetryClient client = new TelemetryClient();
    client.TrackTrace("Hello World!");
```

See Console App [example](samples\ConsoleApp\Program.cs#L11)
