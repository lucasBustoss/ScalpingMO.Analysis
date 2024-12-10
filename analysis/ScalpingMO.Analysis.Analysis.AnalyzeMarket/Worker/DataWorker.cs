using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Data;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.Betfair;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Worker
{
    public class DataWorker
    {
        private MongoDBService _mongoDB;
        private FixtureInfo _fixtureInfo;

        public DataWorker(MongoDBService mongoDB)
        {
            _mongoDB = mongoDB;
        }

        public async Task Execute(FixtureInfo fixtureInfo)
        {
            _fixtureInfo = fixtureInfo;
        }

        #region Private methods

        private List<BetfairInfo> GetBetfairInfoAsync()
        {
            List<BetfairInfo> betfairInfo = _mongoDB.GetBetfairInfoToAnalyze(Convert.ToInt32(_fixtureInfo.BetfairId));
            return betfairInfo.OrderBy(f => f.DateTime).ToList();
        }

        private List<BetfairInfoOdds> FilterOdds(List<BetfairInfoOdds> oddsList)
        {
            var index = oddsList.FindIndex(odds => odds.IsLastCorrespondence);

            if (index == -1)
                return oddsList;

            int start = Math.Max(0, index - 10);
            int end = Math.Min(oddsList.Count - 1, index + 10);

            return oddsList.GetRange(start, end - start + 1);
        }

        #endregion
    }
}
