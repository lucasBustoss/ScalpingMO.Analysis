using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Odds
{
    public class OddsDataResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public int Name { get; set; }

        [JsonPropertyName("values")]
        public List<OddsValueResponse> Odds { get; set; }
    }
}
