namespace ScalpingMO.Analysis.Analysis.ConsolidateData.Interfaces
{
    public interface IFixture
    {
        public int Id { get; }
        public string HomeTeam { get; }
        public string AwayTeam { get; }
        public DateTime Date { get; }
    }
}
