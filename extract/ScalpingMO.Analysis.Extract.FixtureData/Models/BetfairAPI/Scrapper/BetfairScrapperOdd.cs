namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper
{
    public class BetfairScrapperOdd
    {
        public BetfairScrapperOdd(string odd, string availableToLay, string availableToBack, bool isLastCorrespondence)
        {
            Odd = odd;
            AvailableToLay = availableToLay;
            AvailableToBack = availableToBack;
            IsLastCorrespondence = isLastCorrespondence;
        }

        public string Odd { get; set; }
        public string AvailableToLay { get; set; }
        public string AvailableToBack { get; set; }
        public bool IsLastCorrespondence { get; set; }
    }
}
