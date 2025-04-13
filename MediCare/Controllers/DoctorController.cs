using MediCare.Data;
using MediCare.Models;
using MediCare.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;
using System.Drawing;
using SixLabors.ImageSharp;


namespace MediCare.Controllers
{
    public class DoctorController : Controller
    {

        private readonly MongoDbContext _dbcontext;
        private readonly IMongoCollection<Review> _reviewCollection;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DoctorController(MongoDbContext dbcontext, IWebHostEnvironment webHostEnvironment)
        {
            _dbcontext = dbcontext;
            _reviewCollection = dbcontext.Reviews;
            _webHostEnvironment = webHostEnvironment;
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
            doctorDetailsViewModel.DoctorDesc = doctor.Description;
            var specialization = await _dbcontext.Specializations.Find(s => s.ObjectId == doctor.SpecializationId).FirstOrDefaultAsync();
            doctorDetailsViewModel.Specialization = specialization != null ? specialization.SpecializationName : "Unknown";
            doctorDetailsViewModel.DoctorID = doctor.ObjectId;
            doctorDetailsViewModel.Appointments = appointmentsLst;
            doctorDetailsViewModel.DoctorImage = doctor.DoctorImage;
           
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
               if(appointment.PatientId != null)
                {
                    var patient = await _dbcontext.Patients.Find(s => s.ObjectId == appointment.PatientId).FirstOrDefaultAsync();
                    appointmentsViewModel.PatientName = patient.Name;
                }
               
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
                doctorInfo.Description = doctor.Description;    
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
            newDoctor.Description = doctorViewModel.Description;
            newDoctor.DoctorImage = SaveFileFromBase64(doctorViewModel.DoctorImage, _webHostEnvironment);
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
            newAppointment.Description = doctorDetailsViewModel.AppointmentDescription;
            Patient patient = await _dbcontext.Patients.Find(s => s.Name == doctorDetailsViewModel.PatientName 
            && s.Email == doctorDetailsViewModel.Email
            && s.Phone == doctorDetailsViewModel.PatientPhone ).FirstOrDefaultAsync();
            if (patient != null)
            {
              newAppointment.PatientId = patient.ObjectId;
            }
            else
            {
                Patient newPatient = new Patient();
                newPatient.Email = doctorDetailsViewModel.PatientEmail;
                newPatient.Age = doctorDetailsViewModel.PatientAge;
                newPatient.Phone = doctorDetailsViewModel.PatientPhone; 
                newPatient.Name = doctorDetailsViewModel.PatientName;
                await _dbcontext.Patients.InsertOneAsync(newPatient);
                newAppointment.PatientId = newPatient.ObjectId;
            }
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


        private string SaveFileFromBase64(string base64String, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String) || !base64String.Contains(","))
                    return null;

                // Extract the base64 data part
                byte[] fileBytes = Convert.FromBase64String(base64String.Split(',')[1]);

                // Generate a unique filename
                string fileName = $"{Guid.NewGuid()}.jpg";

                // Use web root path instead of AppDomain.CurrentDomain.BaseDirectory
                string directoryPath = Path.Combine(webHostEnvironment.WebRootPath, "Images", "Doctor");

                // Create directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, fileName);

                // Using SixLabors.ImageSharp
                using (var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(fileBytes))
                {
                    // Set jpg encoding options with quality
                    var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                    {
                        Quality = 50 // 50% quality compression
                    };

                    // Save the image - using the correct overload
                    image.Save(filePath, encoder);
                }

                // Return the relative URL path for the saved image
                return $"/Images/Doctor/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }

    }
}
