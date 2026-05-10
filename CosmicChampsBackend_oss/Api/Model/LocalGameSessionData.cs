using System.Text.Json.Serialization;

namespace CosmicChamps.Api.Model;

public class LocalGameSessionData
{
    [JsonConverter (typeof (JsonStringEnumConverter))]
    public GameMode GameMode { set; get; }

    public string? TournamentId { set; get; }

    public List<string> PlayerIds { set; get; }

    public string MatchmakingConfigurationArn { set; get; }
}