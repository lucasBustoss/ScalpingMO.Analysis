using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response
{
    public class BetfairMarketBookRunnersResponse
    {
        [JsonPropertyName("selectionId")]
        public int SelectionId { get; set; }

        [JsonPropertyName("lastPriceTraded")]
        public double LastPriceTraded { get; set; }

        [JsonPropertyName("ex")]
        public BetfairMarketBookRunnerDataResponse RunnerOdds { get; set; }
    }
}
