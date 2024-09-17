using ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models
{
    public class Fixture
    {
        public Fixture()
        {

        }

        public Fixture(int id, DateTime date, string marketId, string homeTeamName, string awayTeamName)
        {
            Id = id;
            Date = date;
            MarketId = marketId;
            HomeTeam = homeTeamName;
            AwayTeam = awayTeamName;
        }

        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime Date { get; set; }
        public string? MarketId { get; set; }
    }
}
