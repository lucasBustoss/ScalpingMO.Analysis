using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response
{
    public class BetfairEventDescription
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("openDate")]
        public string OpenDate { get; set; }
    }
}
