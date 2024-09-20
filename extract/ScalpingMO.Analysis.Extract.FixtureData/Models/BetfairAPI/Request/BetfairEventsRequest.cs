using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Request
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
