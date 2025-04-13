using MediCare.Data;
using MediCare.Models;
using MediCare.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MediCare.Controllers
{
    public class DoctorController : Controller
    {

        private readonly MongoDbContext _dbcontext;
        private readonly IMongoCollection<Review> _reviewCollection;


        public DoctorController(MongoDbContext dbcontext)
        {
            _dbcontext = dbcontext;
            _reviewCollection = dbcontext.Reviews;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Booking(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var doctor = await _dbcontext.Doctors.Find(s => s.ObjectId == id).FirstOrDefaultAsync();
            if (doctor == null)
            {
                return NotFound();
            }
            List<Appointment> appointmentsLst = await _dbcontext.Appointments.Find(s => s.DoctorId == id).ToListAsync();
            DoctorDetailsViewModel doctorDetailsViewModel = new DoctorDetailsViewModel();
            doctorDetailsViewModel.StartTime = doctor.StartTime.ToString()  ;
            doctorDetailsViewModel.EndTime = doctor.EndTime.ToString();
            doctorDetailsViewModel.Degree = doctor.Degree.ToString() ;
            doctorDetailsViewModel.Email = doctor.Email.ToString() ;
            doctorDetailsViewModel.Name = doctor.Name.ToString() ;
            var specialization = await _dbcontext.Specializations.Find(s => s.ObjectId == doctor.SpecializationId).FirstOrDefaultAsync();
            doctorDetailsViewModel.Specialization = specialization != null ? specialization.SpecializationName : "Unknown";
            doctorDetailsViewModel.DoctorID = doctor.ObjectId;
            doctorDetailsViewModel.Appointments = appointmentsLst;
           
            return View(doctorDetailsViewModel);
        
        }


        [HttpGet]
        public async Task<IActionResult> DoctorAppointments(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            List<Appointment> appointmentsLst = await _dbcontext.Appointments.Find(s => s.DoctorId == id).ToListAsync();
            if(appointmentsLst == null)
            {
                return NotFound();
            }
            var doctor = await _dbcontext.Doctors.Find(s => s.ObjectId == id).FirstOrDefaultAsync();
            if (doctor == null)
            {
                return NotFound();
            }
            var specialization = await _dbcontext.Specializations.Find(s => s.ObjectId == doctor.SpecializationId).FirstOrDefaultAsync();
            
            List<AppointmentsViewModel> appointmentsViewModelList = new List<AppointmentsViewModel>();
            foreach (Appointment appointment in appointmentsLst)
            {
                AppointmentsViewModel appointmentsViewModel = new AppointmentsViewModel();
                appointmentsViewModel.AppId = appointment.ObjectId;
                appointmentsViewModel.DoctorId = doctor.ObjectId;
                appointmentsViewModel.DoctorName = doctor.Name;
                appointmentsViewModel.BookedTime = appointment.BookedTime.ToString();
                appointmentsViewModel.BookedDate = appointment.BookedDate.ToString();
                appointmentsViewModel.Specialization = specialization != null ? specialization.SpecializationName : "Unknown";
                appointmentsViewModelList.Add(appointmentsViewModel);
            }
            
            return View(appointmentsViewModelList);

        }



        [HttpGet]
        public async Task<IActionResult> DoctorPage()
        {
            DoctorViewModel doctorViewModel = new DoctorViewModel();
            doctorViewModel.SpecializationList = await _dbcontext.Specializations.Find(_ => true).ToListAsync();
            List<Doctor> doctors = await _dbcontext.Doctors.Find(_ => true).ToListAsync();
            doctorViewModel.DoctorInfos = new List<DoctorInfo>();
            int i = 1;
            foreach(Doctor doctor in doctors)
            {
              DoctorInfo doctorInfo = new DoctorInfo();
                doctorInfo.ObjectId = doctor.ObjectId;
                doctorInfo.Id = i++;
              doctorInfo.Name = doctor.Name;
                doctorInfo.Email    = doctor.Email;
                doctorInfo.Degree = doctor.Degree;
                doctorInfo.StartTime = doctor.StartTime.ToString(); 
                doctorInfo.EndTime = doctor.EndTime.ToString();
                var specialization = await _dbcontext.Specializations.Find(s => s.ObjectId == doctor.SpecializationId).FirstOrDefaultAsync();
                doctorInfo.Specialization = specialization != null ? specialization.SpecializationName : "Unknown";
                doctorViewModel.DoctorInfos.Add(doctorInfo);
            }
            return View(doctorViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorViewModel doctorViewModel)
        {
            Doctor newDoctor = new Doctor();
            newDoctor.SpecializationId = doctorViewModel.Specialization;
            newDoctor.Name = doctorViewModel.Name;
            newDoctor.Age = doctorViewModel.Age;
            newDoctor.Email = doctorViewModel.Email;
            newDoctor.Degree = doctorViewModel.Degree;
            if (TimeOnly.TryParse(doctorViewModel.StartTime, out TimeOnly startTime))
                newDoctor.StartTime = startTime;

            if (TimeOnly.TryParse(doctorViewModel.EndTime, out TimeOnly endTime))
                newDoctor.EndTime = endTime;
            await _dbcontext.Doctors.InsertOneAsync(newDoctor);
            // AddAvailabitySlots(newDoctor);
            return RedirectToAction("DoctorPage");
        }

        //public void AddAvailabitySlots(Doctor doctor)
        //{

        //}

        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] DoctorDetailsViewModel doctorDetailsViewModel)
        {
            
           Appointment newAppointment = new Appointment();
            newAppointment.DoctorId = doctorDetailsViewModel.DoctorID;
            if (TimeOnly.TryParse(doctorDetailsViewModel.Bookedtime, out TimeOnly bookedTime))
                newAppointment.BookedTime = bookedTime;
            if (DateOnly.TryParse(doctorDetailsViewModel.BookedDate, out DateOnly bookedDate))
                newAppointment.BookedDate = bookedDate;
            await _dbcontext.Appointments.InsertOneAsync(newAppointment);
            // AddAvailabitySlots(newDoctor);
            return RedirectToAction("DoctorPage");
        }
        [HttpPost]
        public async Task<IActionResult> SubmitReview([FromBody] Review review)
        {
            review.UpdatedDate = DateTime.Now;
            await _reviewCollection.InsertOneAsync(review);
            return Ok(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctorReviews(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
                return BadRequest("Doctor ID is required");

            var reviews = await _reviewCollection.Find(r => r.DoctorId == doctorId).ToListAsync();
            return Json(reviews);
        }

    }
}
