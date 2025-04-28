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

        [BsonElement("Prescription")]
        public string PrescriptionText { get; set; }


        [BsonElement("Medicines")]
        public List<string> Medicines { get; set; }

        [BsonElement("Tests")]
        public List<string> Tests { get; set; }

        [BsonElement("NeedRevisit")]
        public bool NeedRevisit { get; set; }

        [BsonElement("PrescriptionDate")]
        public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;
    }

}
