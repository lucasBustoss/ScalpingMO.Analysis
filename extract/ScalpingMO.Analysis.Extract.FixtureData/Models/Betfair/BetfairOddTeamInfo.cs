using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI
{
    public class BetfairOddTeamInfo
    {
        public BetfairOddTeamInfo(BetfairMarketBookRunnersResponse teamData, string teamName)
        {
            TeamId = teamData.SelectionId;
            Name = teamName;
            LastPriceTraded = teamData.LastPriceTraded;

            AvailableToBack = new List<BetfairOddAvailableInfo>();
            foreach (BetfairMarketBookOddResponse odd in teamData.RunnerOdds.Back)
            {
                BetfairOddAvailableInfo oddToInsert = new BetfairOddAvailableInfo(odd);
                AvailableToBack.Add(oddToInsert);
            }

            AvailableToLay = new List<BetfairOddAvailableInfo>();
            foreach (BetfairMarketBookOddResponse odd in teamData.RunnerOdds.Lay)
            {
                BetfairOddAvailableInfo oddToInsert = new BetfairOddAvailableInfo(odd);
                AvailableToLay.Add(oddToInsert);
            }
        }

        public int TeamId { get; set; }
        public string Name { get; set; }
        public double LastPriceTraded { get; set; }
        public List<BetfairOddAvailableInfo> AvailableToBack { get; set; }
        public List<BetfairOddAvailableInfo> AvailableToLay {  get; set; }
    }
}
