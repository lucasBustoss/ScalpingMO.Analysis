namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper
{
    public class BetfairScrapperTeamOdd
    {
        public BetfairScrapperTeamOdd(string minute)
        {
            Odds = new List<BetfairScrapperOdd>();
        }

        public List<BetfairScrapperOdd> Odds { get; set; }
    }
}
