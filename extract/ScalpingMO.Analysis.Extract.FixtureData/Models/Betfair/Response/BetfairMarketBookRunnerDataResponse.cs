using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response
{
    public class BetfairMarketBookRunnerDataResponse
    {
        [JsonPropertyName("availableToBack")]
        public List<BetfairMarketBookOddResponse> Back { get; set; }

        [JsonPropertyName("availableToLay")]
        public List<BetfairMarketBookOddResponse> Lay { get; set; }
    }
}
