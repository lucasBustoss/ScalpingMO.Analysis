using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models
{
    public class RateLimit
    {
        public RateLimit(string limit, string remaining, string reset)
        {
            RequestsLimit = Convert.ToInt32(limit);
            RequestsRemaining = Convert.ToInt32(remaining);
            RequestsReset = Convert.ToInt32(reset);
            LastRequest = DateTime.UtcNow;
        }

        [BsonId] 
        public ObjectId Id { get; set; }

        public int RequestsLimit { get; set; }
        public int RequestsRemaining { get; set; }
        public int RequestsReset { get; set; }
        public DateTime LastRequest { get; set; }
    }
}
