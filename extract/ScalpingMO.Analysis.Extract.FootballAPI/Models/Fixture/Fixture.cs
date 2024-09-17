using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Fixture;
using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Teams;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Fixture
{
    public class Fixture
    {
        public Fixture()
        {
            
        }

        public Fixture(FixtureDataResponse fixture, FixtureTeamDataResponse homeTeam, FixtureTeamDataResponse awayTeam)
        {
            Id = fixture.Id;
            Date = DateTimeOffset.FromUnixTimeSeconds(fixture.Timestamp).UtcDateTime;

            HomeTeam = new FixtureTeam(homeTeam);
            AwayTeam = new FixtureTeam(awayTeam);
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public FixtureTeam HomeTeam { get; set; }
        public FixtureTeam AwayTeam { get; set; }
    }
}
