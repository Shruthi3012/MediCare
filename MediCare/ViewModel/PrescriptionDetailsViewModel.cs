using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MediCare.Models;

namespace MediCare.ViewModel
{
    public class PrescriptionDetailsViewModel
    {
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string DoctorName { get; set; }
        public int DoctorAge {  get; set; }
        public string DoctorEmail {  get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorDegree { get; set; }
        public string DoctorDescription { get; set; }
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        public string PatientEmail { get; set; }
        public string PhoneNumber {  get; set; }
        public string BookedTime { get; set; }

        public string BookedDate { get; set; }
        public string Description { get; set; }
        public string AppId { get; set; }
        public string Prescription { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public int? Rating { get; set; } // Use nullable int for rating
        public string Comments { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
