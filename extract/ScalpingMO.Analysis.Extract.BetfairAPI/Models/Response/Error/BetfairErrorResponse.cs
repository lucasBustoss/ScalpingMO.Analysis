using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response.Error
{
    public class BetfairErrorResponse
    {
        [JsonPropertyName("faultstring")]
        public string Code { get; set; }

        [JsonPropertyName("detail")]
        public BetfairErrorDetail Detail { get; set; }
    }
}
