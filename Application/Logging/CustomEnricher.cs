using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Application.Logging;

public class CustomEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (Activity.Current is null)
        {
            var activity = new Activity("Logging");

            activity.Start();

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("RequestId", activity.TraceId.ToString()));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ParentId", activity.ParentId));
        }

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("RequestId", Activity.Current!.TraceId.ToString()));
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ParentId", Activity.Current.ParentId));
    }
}