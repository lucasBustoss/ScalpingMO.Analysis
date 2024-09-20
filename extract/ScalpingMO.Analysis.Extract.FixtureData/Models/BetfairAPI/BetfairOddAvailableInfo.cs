using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI
{
    public class BetfairOddAvailableInfo
    {
        public BetfairOddAvailableInfo(BetfairMarketBookOddResponse odd)
        {
            Odd = odd.Odd;
            Available = odd.TotalAvailable;
        }

        public double Odd { get; set; }
        public double Available { get; set; } 
    }
}
