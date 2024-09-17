using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Fixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response
{
    public class OddsResponse
    {
        [JsonPropertyName("fixture")]
        public FixtureDataResponse Fixture { get; set; }
    }
}
