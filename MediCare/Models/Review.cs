using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediCare.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }

        [BsonElement("doctor_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DoctorId { get; set; }

        [BsonElement("patient_id")]
        public string PatientId { get; set; }

        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("comments")]
        public string Comments { get; set; }

        [BsonElement("updated_date")]
        public DateTime UpdatedDate { get; set; }
        
        [BsonElement("appId")]
        public string AppId { get; set; }
    }
}
