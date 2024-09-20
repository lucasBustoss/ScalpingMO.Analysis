using ScalpingMO.Analysis.Analysis.ConsolidateData.Interfaces;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Models
{
    public class BetfairFixture : IFixture
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int? HomeTeamId { get; set; }
        public int? AwayTeamId { get; set; }
        public DateTime Date { get; set; }
        public string? MarketId { get; set; }
    }
}
