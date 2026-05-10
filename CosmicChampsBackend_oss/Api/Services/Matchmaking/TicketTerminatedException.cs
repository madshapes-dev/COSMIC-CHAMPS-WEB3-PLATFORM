using System.Runtime.Serialization;

namespace CosmicChamps.Api.Services.Matchmaking;

public class TicketTerminatedException : Exception
{
    public TicketTerminationReason Reason { private set; get; }

    public TicketTerminatedException (TicketTerminationReason reason)
    {
        Reason = reason;
    }

    protected TicketTerminatedException (
        SerializationInfo info,
        StreamingContext context,
        TicketTerminationReason reason) : base (info, context)
    {
        Reason = reason;
    }

    public TicketTerminatedException (string? message, TicketTerminationReason reason) : base (message)
    {
        Reason = reason;
    }

    public TicketTerminatedException (string? message, Exception? innerException, TicketTerminationReason reason) : base (
        message,
        innerException)
    {
        Reason = reason;
    }
}