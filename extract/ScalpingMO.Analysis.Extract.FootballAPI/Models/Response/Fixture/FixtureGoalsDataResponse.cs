using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Fixture
{
    public class FixtureGoalsDataResponse
    {
        [JsonPropertyName("home")]
        public int? HomeGoals { get; set; }

        [JsonPropertyName("away")]
        public int? AwayGoals { get; set; }
    }
}
