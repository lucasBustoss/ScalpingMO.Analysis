
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
            DateTime = DateTime.UtcNow;
        }

        public ObjectId Id { get; set; }
        public string MatchName { get; set; }
        public int EventId { get; set; }
        public string MarketId { get; set; }
        public DateTime DateTime { get; set; }
        public string Minute { get; set; }
        public List<BetfairScrapperOdd> HomeOdds { get; set; }
        public List<BetfairScrapperOdd> DrawOdds { get; set; }
        public List<BetfairScrapperOdd> AwayOdds { get; set; }
    }
}
