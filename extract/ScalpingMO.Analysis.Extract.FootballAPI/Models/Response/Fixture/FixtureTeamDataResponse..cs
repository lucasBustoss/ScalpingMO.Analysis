using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Teams
{
    public class FixtureTeamDataResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("logo")]
        public string Logo { get; set; }
    }
}
