using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds
{
    public class OddsResponse
    {
        [JsonPropertyName("fixture")]
        public FixtureDataResponse Fixture { get; set; }

        [JsonPropertyName("odds")]
        public List<OddsDataResponse> Odds { get; set; }
    }
}
