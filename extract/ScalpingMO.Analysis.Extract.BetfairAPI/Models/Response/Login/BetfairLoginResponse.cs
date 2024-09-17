using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response.Login
{
    public class BetfairLoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
