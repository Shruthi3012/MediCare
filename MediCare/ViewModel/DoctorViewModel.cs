using MediCare.Models;

namespace MediCare.ViewModel
{
    public class DoctorViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public List<Specialization> SpecializationList { get; set; }
        public string Specialization {  get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Degree { get; set; }  

    }
}
