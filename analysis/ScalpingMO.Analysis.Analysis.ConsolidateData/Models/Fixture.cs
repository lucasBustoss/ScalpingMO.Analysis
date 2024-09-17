using MongoDB.Bson;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Models
{
    public class Fixture
    {
        public Fixture()
        {

        }

        public Fixture(DateTime date, string status, int footballApiId, string homeTeamName, string homeTeamLogo, int homeTeamGoals, string awayTeamName, string awayTeamLogo, int awayTeamGoals)
        {
            Id = ObjectId.GenerateNewId();
            Status = status;
            Date = date;
            FootballApiId = footballApiId;
            HomeTeamName = homeTeamName;
            HomeTeamLogo = homeTeamLogo;
            HomeTeamGoals = homeTeamGoals;
            AwayTeamName = awayTeamName;
            AwayTeamLogo = awayTeamLogo;
            AwayTeamGoals = awayTeamGoals;
        }

        public ObjectId Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public int FootballApiId { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamLogo { get; set; }
        public int HomeTeamGoals { get; set; }
        public string AwayTeamName { get; set; }
        public string AwayTeamLogo { get; set; }
        public int AwayTeamGoals { get; set; }
        public string? MarketId { get; set; }
        public string? RadarUrl { get; set; }
        public int? BetfairId { get; set; }
        public int? WilliamHillId { get; set; }
        public bool ShouldAnalyze { get; set; }
    }
}
