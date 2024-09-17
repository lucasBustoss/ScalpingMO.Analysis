using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Fixture
{
    public class FixtureStatusDataResponse
    {
        [JsonPropertyName("short")]
        public string Value { get; set; }
    }
}
