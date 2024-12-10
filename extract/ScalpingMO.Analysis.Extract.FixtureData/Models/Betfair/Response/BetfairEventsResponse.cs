using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response
{
    public class BetfairEventResponse
    {
        [JsonPropertyName("event")]
        public BetfairEventDescription Event { get; set; }

        [JsonPropertyName("marketCount")]
        public int MarketCount { get; set; }
    }
}
