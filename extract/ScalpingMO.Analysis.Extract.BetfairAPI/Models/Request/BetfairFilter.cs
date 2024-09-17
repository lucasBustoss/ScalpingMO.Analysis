using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Request
{
    public class BetfairFilter
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("eventIds")]
        public List<string> EventIds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("eventTypeIds")]
        public List<string> EventTypeIds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("marketTypeCodes")]
        public List<string> MarketTypeCodes { get; set; }
    }
}
