using MediCare.Models;

namespace MediCare.ViewModel
{
    public class DoctorDetailsViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Degree { get; set; }
        public string DoctorDesc { get; set; }

        public string DoctorID { get; set; }
        public string DoctorImage { get; set; }

        public string Bookedtime { get; set; }
        public string BookedDate { get; set; }

        public string Status { get; set; }
        public string AppointmentDescription { get; set; } 

        public List<Appointment> Appointments { get; set; }

        public string PatientName { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }
        public int PatientAge { get; set; }


    }
}
