using MongoDB.Bson;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI
{
    public class BetfairOddInfo
    {
        public BetfairOddInfo(int eventId, BetfairMarketBookResponse betfairResponse, int homeSelectionId, int awaySelectionId, string homeTeamName, string awayTeamName)
        {
            Id = ObjectId.GenerateNewId();
            BetfairId = eventId;
            MarketId = betfairResponse.MarketId;
            BetDelay = betfairResponse.BetDelay;
            TotalMatched = betfairResponse.TotalMatched;
            LastMatchedTime = betfairResponse.LastMatchedTime;

            BetfairMarketBookRunnersResponse homeTeamData = betfairResponse.Runners.Where(r => r.SelectionId == homeSelectionId).FirstOrDefault();
            BetfairMarketBookRunnersResponse awayTeamData = betfairResponse.Runners.Where(r => r.SelectionId == awaySelectionId).FirstOrDefault();

            if (homeTeamData != null && awayTeamData != null)
            {
                HomeTeam = new BetfairOddTeamInfo(homeTeamData, homeTeamName);
                AwayTeam = new BetfairOddTeamInfo(awayTeamData, awayTeamName);
            }
        }

        public ObjectId Id { get; set; }
        public int BetfairId { get; set; }
        public string MarketId { get; set; }
        public int BetDelay { get; set; }
        public double TotalMatched { get; set; }
        public DateTime LastMatchedTime { get; set; }

        public BetfairOddTeamInfo HomeTeam { get; set; }
        public BetfairOddTeamInfo AwayTeam { get; set; }
    }
}
