using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediCare.ViewModel
{
    public class PrescriptionDetailsViewModel
    {
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Degree { get; set; }
        public string BookedTime { get; set; }

        public string BookedDate { get; set; }
        public string Description { get; set; }
        public string AppId { get; set; }
        public string Prescription { get; set; }
        public List<string> Medicines { get; set; }
        public List<string> Tests { get; set; }
        public bool NeedRevisit { get; set; }
        public DateTime PrescriptionDate { get; set; }
    }
}
