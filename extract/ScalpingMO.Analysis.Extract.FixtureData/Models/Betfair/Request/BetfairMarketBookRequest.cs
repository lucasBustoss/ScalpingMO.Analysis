using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Request
{
    public class BetfairMarketBookRequest
    {
        [JsonPropertyName("marketIds")]
        public List<string> MarketIds { get; set; }

        [JsonPropertyName("priceProjection")]
        public PriceProjection PriceProjection { get; set; }
    }

    public class PriceProjection
    {
        [JsonPropertyName("priceData")]
        public List<string> PriceData { get; set; }
    }
}
