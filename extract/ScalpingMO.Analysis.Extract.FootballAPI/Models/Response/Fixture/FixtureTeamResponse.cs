using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Teams
{
    public class FixtureTeamResponse
    {
        [JsonPropertyName("home")]
        public FixtureTeamDataResponse HomeTeam { get; set; }

        [JsonPropertyName("away")]
        public FixtureTeamDataResponse AwayTeam { get; set; }
    }
}
