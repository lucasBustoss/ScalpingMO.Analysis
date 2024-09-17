using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Odds
{
    public class OddsValueResponse
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("odd")]
        public double Odd { get; set; }
    }
}
