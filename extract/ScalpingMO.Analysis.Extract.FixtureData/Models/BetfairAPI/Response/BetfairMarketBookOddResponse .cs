using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response
{
    public class BetfairMarketBookOddResponse
    {
        [JsonPropertyName("price")]
        public double Odd { get; set; }

        [JsonPropertyName("size")]
        public double TotalAvailable { get; set; }
    }
}
