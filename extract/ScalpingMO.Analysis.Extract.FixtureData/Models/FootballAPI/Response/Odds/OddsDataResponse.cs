using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds
{
    public class OddsDataResponse
    {
        [JsonPropertyName("values")]
        public List<OddsDataValueResponse> Values { get; set; }
    }
}
