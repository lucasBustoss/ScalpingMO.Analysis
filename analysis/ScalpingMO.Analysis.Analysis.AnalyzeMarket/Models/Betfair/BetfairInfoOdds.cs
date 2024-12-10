namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.Betfair
{
    public class BetfairInfoOdds
    {
        public string Odd { get; set; }
        public string AvailableToLay { get; set; }
        public string AvailableToBack { get; set; }
        public bool IsLastCorrespondence { get; set; }
    }
}
