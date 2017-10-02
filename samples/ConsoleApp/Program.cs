using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceContext;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ServiceContextTelemetryIntitializer());
            TelemetryClient client = new TelemetryClient();

            client.TrackTrace("Hello World!");
        }
    }
}
