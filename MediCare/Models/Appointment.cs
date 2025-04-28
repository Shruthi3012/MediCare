using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediCare.Models
{
    public class Appointment
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string DoctorId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }

        [BsonElement("BookedTime")]
        public TimeOnly BookedTime { get; set; }

        [BsonElement("BookedDate")]
        public DateOnly BookedDate { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Prescription")]
        public string Prescription { get; set; }


    }
}
