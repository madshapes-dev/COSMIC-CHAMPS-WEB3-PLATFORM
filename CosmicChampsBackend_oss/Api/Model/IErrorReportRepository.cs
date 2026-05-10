namespace CosmicChamps.Api.Model;

public interface IErrorReportRepository
{
    Task CreateAsync (ErrorReport errorReport);
}