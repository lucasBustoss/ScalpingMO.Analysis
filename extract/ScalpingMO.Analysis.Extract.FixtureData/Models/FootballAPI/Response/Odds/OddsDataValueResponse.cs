using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds
{
    public class OddsDataValueResponse
    {
        [JsonPropertyName("value")]
        public string Side { get; set; }

        [JsonPropertyName("odd")]
        public string Odd { get; set; }
    }
}
