using MongoDB.Bson;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Models
{
    public class Fixture
    {
        public Fixture()
        {
            
        }

        public Fixture(DateTime date, int footballApiId, string homeTeamName, string homeTeamLogo, string awayTeamName, string awayTeamLogo)
        {
            Id = ObjectId.GenerateNewId();
            Date = date;
            FootballApiId = footballApiId;
            HomeTeamName = homeTeamName;
            HomeTeamLogo = homeTeamLogo;
            AwayTeamName = awayTeamName;
            AwayTeamLogo = awayTeamLogo;
        }

        public ObjectId Id { get; set; }
        public DateTime Date { get;set; }
        public int FootballApiId { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamLogo { get; set; }
        public string AwayTeamName { get; set; }
        public string AwayTeamLogo { get; set; }
        public string? MarketId { get; set; }
        public string? RadarUrl { get; set; }
        public int? BetfairId { get; set; }
        public int? WilliamHillId { get; set; }
    }
}
