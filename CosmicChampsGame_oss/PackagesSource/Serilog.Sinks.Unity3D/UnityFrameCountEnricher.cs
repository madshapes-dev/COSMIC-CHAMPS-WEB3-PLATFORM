using System;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using UnityEngine;

namespace Serilog
{
    public sealed class UnityFrameCountEnricher : ILogEventEnricher
    {
        private class FrameCountLogEventPropertyValue : LogEventPropertyValue
        {
            public override void Render (TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
            {
                output.Write (Time.frameCount.ToString ());
            }
        }        
        
        public const string PropertyName = "UnityFrameCount";

        private readonly LogEventProperty _property;

        public UnityFrameCountEnricher () =>
            _property = new LogEventProperty (PropertyName, new FrameCountLogEventPropertyValue ());

        public void Enrich (LogEvent logEvent, ILogEventPropertyFactory propertyFactory) =>
            logEvent.AddPropertyIfAbsent (_property);
    }
}