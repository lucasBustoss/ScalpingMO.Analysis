using MongoDB.Bson;
using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Enums;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models
{
    public class FixtureMarketAnalyze
    {
        public ObjectId Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public MarketTendencyEnum HomeMarketTendency { get; set; }
        public MarketTendencyEnum AwayMarketTendency { get;set; }
        public double LastBetfairOdd { get; set; }
        public double LastReferenceOdd { get; set; }
    }
}
