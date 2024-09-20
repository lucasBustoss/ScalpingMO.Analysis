using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response
{
    public class BetfairMarketCatalogueRunner
    {
        [JsonPropertyName("selectionId")]
        public int SelectionId { get; set; }

        [JsonPropertyName("runnerName")]
        public string RunnerName { get; set; }
    }
}
