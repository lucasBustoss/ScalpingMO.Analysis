using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Fixture;
using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Teams;
using System.Text.Json.Serialization;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Response
{
    public class FixtureResponse
    {
        [JsonPropertyName("fixture")]
        public FixtureDataResponse Fixture { get; set; }

        [JsonPropertyName("teams")]
        public FixtureTeamResponse Teams { get; set; }

    }
}
