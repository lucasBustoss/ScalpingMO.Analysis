using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response.Error
{
    public class BetfairErrorDetail
    {
        [JsonPropertyName("APINGException")]
        public BetfairErrorDetailException Exception { get; set; }
    }
}
