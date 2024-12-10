using MongoDB.Bson;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Models.WilliamHill
{
    public class WilliamHillInfo
    {
        public ObjectId Id { get; set; }
        public int WilliamHillId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Minute { get; set; }
        public string Description { get; set; }
    }
}
