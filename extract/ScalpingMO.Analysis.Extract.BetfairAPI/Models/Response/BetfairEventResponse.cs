using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response
{
    public class BetfairEventResponse
    {
        [JsonPropertyName("event")]
        public BetfairEventDescription Event { get; set; }

        [JsonPropertyName("marketCount")]
        public int MarketCount { get; set; }
    }
}
