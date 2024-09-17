namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Models
{
    public class FootballApiFixture
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public FootballApiTeam HomeTeam { get; set; }
        public FootballApiTeam AwayTeam { get; set; }
    }
}
