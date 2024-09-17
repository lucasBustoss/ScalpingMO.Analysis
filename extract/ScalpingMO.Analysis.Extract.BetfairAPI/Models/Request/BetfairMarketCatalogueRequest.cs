using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Request
{
    public class BetfairListMarketCatalogueRequest
    {
        public BetfairListMarketCatalogueRequest()
        {
            Filter = new BetfairFilter();
        }

        [JsonPropertyName("filter")]
        public BetfairFilter Filter { get; set; }

        [JsonPropertyName("marketProjection")]
        public List<string> MarketProjection { get; set; }

        [JsonPropertyName("maxResults")]
        public int MaxResults { get; set; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
    }
}
