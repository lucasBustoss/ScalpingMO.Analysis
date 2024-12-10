using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response.Error
{
    public class BetfairErrorDetailException
    {
        [JsonPropertyName("errorCode")]
        public string ExceptionCode { get; set; }

        [JsonPropertyName("errorDetails")]
        public string ExceptionDetails { get; set; }
    }
}
