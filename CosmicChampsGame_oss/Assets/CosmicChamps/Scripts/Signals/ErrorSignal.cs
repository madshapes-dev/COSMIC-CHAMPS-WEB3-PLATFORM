using System;

namespace CosmicChamps.Signals
{
    public class ErrorSignal
    {
        public readonly string Message;
        public readonly string Stacktrace;
        public readonly bool Restart;
        public readonly bool HideMessage;
        public readonly bool Reportable;

        public ErrorSignal (Exception exception, bool restart, bool hideMessage, bool reportable = true)
        {
            Message = exception.Message;
            Stacktrace = exception.StackTrace;
            Restart = restart;
            HideMessage = hideMessage;
            Reportable = reportable;
        }

        public ErrorSignal (string message, string stacktrace, bool restart, bool hideMessage, bool reportable = true)
        {
            Message = message;
            Stacktrace = stacktrace;
            Restart = restart;
            HideMessage = hideMessage;
            Reportable = reportable;
        }

        public ErrorSignal (string message, Exception exception, bool restart, bool hideMessage, bool reportable = true)
        {
            Message = message;
            Stacktrace = exception.StackTrace;
            Restart = restart;
            HideMessage = hideMessage;
            Reportable = reportable;
        }

        public static ErrorSignal CreateNonReportable (string message) => new ErrorSignal (
            message,
            string.Empty,
            false,
            false,
            false);
    }
}