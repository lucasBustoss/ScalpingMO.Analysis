using ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Models
{
    public class Fixture
    {
        public Fixture()
        {

        }

        public Fixture(int id, DateTime date, string marketId, string homeTeamName, string awayTeamName, BetfairMarketCatalogueRunner homeRunner, BetfairMarketCatalogueRunner awayRunner)
        {
            Id = id;
            Date = date;
            MarketId = marketId;
            HomeTeam = homeTeamName;
            AwayTeam = awayTeamName;
            HomeTeamId = homeRunner != null ? homeRunner.SelectionId : null;
            AwayTeamId = awayRunner != null ? awayRunner.SelectionId : null; ;
        }

        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public int? HomeTeamId { get; set; }
        public string AwayTeam { get; set; }
        public int? AwayTeamId { get; set; }
        public DateTime Date { get; set; }
        public string? MarketId { get; set; }
    }
}
