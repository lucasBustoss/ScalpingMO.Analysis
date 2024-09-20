
using MongoDB.Bson;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper
{
    public class BetfairScrapperMatch
    {
        public BetfairScrapperMatch(string matchName, int eventId, string marketId)
        {
            Id = ObjectId.GenerateNewId();
            MatchName = matchName;
            EventId = eventId;
            MarketId = marketId;
            Executions = new List<BetfairScrapperExecution>();
        }

        public ObjectId Id { get; set; }
        public string MatchName { get; set; }
        public int EventId { get; set; }
        public string MarketId { get; set; }
        public List<BetfairScrapperExecution> Executions { get; set; }
    }
}
