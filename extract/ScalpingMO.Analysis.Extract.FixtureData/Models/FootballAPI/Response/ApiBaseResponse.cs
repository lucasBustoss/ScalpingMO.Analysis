using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response
{
    public class ApiBaseResponse<T>
    {
        [JsonPropertyName("response")]
        public IEnumerable<T> Response { get; set; }
    }
}
