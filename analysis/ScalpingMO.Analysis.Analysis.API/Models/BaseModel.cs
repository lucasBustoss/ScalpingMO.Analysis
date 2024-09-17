using MongoDB.Bson;

namespace ScalpingMO.Analysis.Analysis.API.Models
{
    public abstract class BaseModel
    {
        public virtual ObjectId Id { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
    }
}
