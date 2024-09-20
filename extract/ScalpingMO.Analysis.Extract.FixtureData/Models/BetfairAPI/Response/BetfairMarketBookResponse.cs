using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response
{
    public class BetfairMarketBookResponse
    {
        [JsonPropertyName("marketId")]
        public string MarketId { get; set; }

        [JsonPropertyName("betDelay")]
        public int BetDelay { get; set; }

        [JsonPropertyName("totalMatched")]
        public double TotalMatched { get; set; }

        [JsonPropertyName("lastMatchTime")]
        public DateTime LastMatchedTime { get; set; }

        [JsonPropertyName("runners")]
        public List<BetfairMarketBookRunnersResponse> Runners { get; set; }
    }
}
