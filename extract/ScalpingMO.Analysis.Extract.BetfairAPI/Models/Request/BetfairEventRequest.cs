using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Request
{
    public class BetfairEventsRequest
    {
        public BetfairEventsRequest()
        {
            Filter = new BetfairFilter();
        }

        [JsonPropertyName("filter")]
        public BetfairFilter Filter { get; set; }
    }
}
