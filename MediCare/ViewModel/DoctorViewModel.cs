using MediCare.Models;

namespace MediCare.ViewModel
{
    public class DoctorViewModel
    {
        public string UpdateDocId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public List<Specialization> SpecializationList { get; set; }
        public string Specialization {  get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Degree { get; set; }

        public string Description { get; set; }

        public string DoctorImage { get; set; }
        public List<DoctorInfo> DoctorInfos { get; set; }

    }

    public class DoctorInfo     
    {
        public string ObjectId { get; set; }
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Degree { get; set; }
        public string Description { get; set; }
        public List<DateTime> bookedAppointments { get; set; }
    }

}
