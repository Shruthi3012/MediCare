using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediCare.Models
{
    public class Prescription
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PrescriptionId { get; set; }

        [BsonElement("DoctorId")]
        public string DoctorId { get; set; }

        [BsonElement("PatientId")]
        public string PatientId { get; set; }

        [BsonElement("AppId")]
        public string AppId { get; set; }

        [BsonElement("Prescription")]
        public string PrescriptionText { get; set; }

        [BsonElement("PrescriptionDate")]
        public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;
    }

}
