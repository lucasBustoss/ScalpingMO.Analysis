using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response.Error
{
    public class BetfairErrorDetail
    {
        [JsonPropertyName("APINGException")]
        public BetfairErrorDetailException Exception { get; set; }
    }
}
