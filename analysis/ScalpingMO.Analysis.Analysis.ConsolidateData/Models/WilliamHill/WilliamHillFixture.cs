using ScalpingMO.Analysis.Analysis.ConsolidateData.Interfaces;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Models
{
    public class WilliamHillFixture : IFixture
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime Date { get; set; }
        public string RadarUrl { get; set; }
    }
}
