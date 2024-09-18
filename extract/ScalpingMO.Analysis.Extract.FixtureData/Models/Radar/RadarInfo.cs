using MongoDB.Bson;

namespace ScalpingMO.Analysis.Extract.FixtureData.Models.Radar
{
    public class RadarInfo
    {
        public RadarInfo(int williamHillId, string minute, string description)
        {
            Id = ObjectId.GenerateNewId();
            WilliamHillId = williamHillId;
            CreationDate = DateTime.UtcNow;
            Minute = minute;
            Description = description;
        }

        public ObjectId Id { get; set; }
        public int WilliamHillId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Minute { get; set; }
        public string Description { get; set; }

        public bool IsEqual(RadarInfo other)
        {
            return
                other != null &&
                Minute == other.Minute &&
                Description == other.Description;
        }
    }
}
