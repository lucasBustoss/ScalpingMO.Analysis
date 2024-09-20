namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper
{
    public class BetfairScrapperTeamOdd
    {
        public BetfairScrapperTeamOdd(string minute)
        {
            Minute = minute;
            Odds = new List<BetfairScrapperOdd>();
        }

        public string Minute { get; set; }
        public List<BetfairScrapperOdd> Odds { get; set; }
    }
}
