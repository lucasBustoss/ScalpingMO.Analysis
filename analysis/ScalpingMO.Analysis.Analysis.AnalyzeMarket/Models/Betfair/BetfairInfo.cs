using MongoDB.Bson;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.Betfair
{
    public class BetfairInfo
    {
        public ObjectId Id { get; set; }
        public string MatchName { get; set; }
        public int EventId { get; set; }
        public string MarketId { get; set; }
        public DateTime DateTime { get; set; }
        public string Minute { get; set; }
        public List<BetfairInfoOdds> HomeOdds { get; set; }
        public List<BetfairInfoOdds> DrawOdds { get; set; }
        public List<BetfairInfoOdds> AwayOdds { get; set; }
    }
}
