using MongoDB.Bson;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI
{
    public class OddResponseRaw
    {
        public ObjectId Id { get; set; }
        public string Content { get; set; }
        public DateTime ExtractionDate { get; set; }
    }
}
