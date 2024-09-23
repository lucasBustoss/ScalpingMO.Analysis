namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper
{
    public class BetfairScrapperExecution
    {
        public BetfairScrapperExecution()
        {
            DateTime = DateTime.UtcNow;
        }

        public DateTime DateTime { get; set; }
        public string Minute { get; set; }
        public BetfairScrapperTeamOdd HomeOdds { get; set; }
        public BetfairScrapperTeamOdd DrawOdds { get; set; }
        public BetfairScrapperTeamOdd AwayOdds { get; set; }

    }
}
