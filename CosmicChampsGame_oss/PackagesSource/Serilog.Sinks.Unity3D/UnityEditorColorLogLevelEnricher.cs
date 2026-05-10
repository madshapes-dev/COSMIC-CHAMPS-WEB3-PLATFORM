using System;
using Serilog.Core;
using Serilog.Events;

namespace Serilog
{
    public sealed class UnityEditorColorLogLevelEnricher : ILogEventEnricher
    {
        public const string PropertyName = "UnityEditorLogLevelColor";

        private readonly string _verboseColor;
        private readonly string _debugColor;
        private readonly string _informationColor;
        private readonly string _warningColor;
        private readonly string _errorColor;

        public UnityEditorColorLogLevelEnricher (
            string verboseColor,
            string debugColor,
            string informationColor,
            string warningColor,
            string errorColor)
        {
            _verboseColor = verboseColor;
            _debugColor = debugColor;
            _informationColor = informationColor;
            _warningColor = warningColor;
            _errorColor = errorColor;
        }

        public void Enrich (LogEvent logEvent, ILogEventPropertyFactory propertyFactory) =>
            logEvent.AddPropertyIfAbsent (
                new LogEventProperty (
                    PropertyName,
                    new ScalarValue (
                        logEvent.Level switch
                        {
                            LogEventLevel.Verbose => _verboseColor,
                            LogEventLevel.Debug => _debugColor,
                            LogEventLevel.Information => _informationColor,
                            LogEventLevel.Warning => _warningColor,
                            LogEventLevel.Error => _errorColor,
                            LogEventLevel.Fatal => _errorColor,
                            _ => throw new ArgumentOutOfRangeException ()
                        })));
    }
}