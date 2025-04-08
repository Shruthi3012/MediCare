using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediCare.Models
{
    public class AvailabilityTime
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public bool IsBooked { get; set; }

        public DateOnly bookedDate {get; set;}

        [BsonRepresentation(BsonType.ObjectId)]
        public string DoctorId { get; set; }
    }
}
