using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediCare.ViewModel
{
    public class AppointmentsViewModel
    {
        public string AppId { get; set; }
        public string DoctorId { get; set; }

        public string DoctorName { get; set; }

        public string Specialization { get; set; } 

        public string BookedTime { get; set; }

        public string BookedDate { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

       public string PatientName { get; set; }

    }
}
