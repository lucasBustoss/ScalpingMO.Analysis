using MongoDB.Bson;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.FootballAPI
{
    public class FootballAPIInfo
    {
        public ObjectId Id { get; set; }
        public int FootballApiId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Minute { get; set; }
        public double HomeOdd { get; set; }
        public double DrawOdd { get; set; }
        public double AwayOdd { get; set; }
    }
}
