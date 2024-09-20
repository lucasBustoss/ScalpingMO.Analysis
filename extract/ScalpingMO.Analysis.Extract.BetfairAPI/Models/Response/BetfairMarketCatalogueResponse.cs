using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response
{
    public class BetfairMarketCatalogueResponse
    {
        [JsonPropertyName("marketId")]
        public string MarketId { get; set; }

        [JsonPropertyName("runners")]
        public List<BetfairMarketCatalogueRunner> Runners { get; set; }
    }
}
