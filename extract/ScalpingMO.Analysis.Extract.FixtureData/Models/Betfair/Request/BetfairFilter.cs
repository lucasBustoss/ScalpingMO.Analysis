using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Request
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
