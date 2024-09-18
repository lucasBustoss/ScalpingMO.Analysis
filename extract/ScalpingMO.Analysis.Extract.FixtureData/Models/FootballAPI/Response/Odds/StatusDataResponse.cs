using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds
{
    public class StatusDataResponse
    {
        [JsonPropertyName("seconds")]
        public string Seconds { get; set; }
    }
}
