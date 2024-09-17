using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Fixture;
using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Teams;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Fixture
{
    public class Fixture
    {
        public Fixture()
        {
            
        }

        public Fixture(FixtureDataResponse fixture, FixtureTeamDataResponse homeTeam, FixtureTeamDataResponse awayTeam, FixtureGoalsDataResponse goals)
        {
            Id = fixture.Id;
            Date = DateTimeOffset.FromUnixTimeSeconds(fixture.Timestamp).UtcDateTime;
            Status = fixture.Status.Value;

            HomeTeam = new FixtureTeam(homeTeam, goals != null && goals.HomeGoals != null ? goals.HomeGoals.Value : 0);
            AwayTeam = new FixtureTeam(awayTeam, goals != null && goals.AwayGoals != null ? goals.AwayGoals.Value : 0);
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public FixtureTeam HomeTeam { get; set; }
        public FixtureTeam AwayTeam { get; set; }
    }
}
