using MongoDB.Bson;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models
{
    public class Sync
    {
        public ObjectId Id { get; set; }
        public string Method { get; set; }
        public DateTime DateTime { get; set; }
    }
}
