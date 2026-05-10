using Serilog.Core;
using Serilog.Events;

namespace Serilog
{
    public sealed class UnityCustomTagEnricher : ILogEventEnricher
    {
        public const string PropertyName = "UnityCustomTag";

        private readonly LogEventProperty _property;

        public UnityCustomTagEnricher (string tag) =>
            _property = new LogEventProperty (PropertyName, new ScalarValue (tag));

        public void Enrich (LogEvent logEvent, ILogEventPropertyFactory propertyFactory) =>
            logEvent.AddPropertyIfAbsent (_property);
    }
}