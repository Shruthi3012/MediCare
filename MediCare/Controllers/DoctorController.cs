using MediCare.Data;
using MediCare.Models;
using MediCare.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;
using System.Drawing;
using SixLabors.ImageSharp;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Globalization;

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
            doctorDetailsViewModel.StartTime = doctor.StartTime.ToString();
            doctorDetailsViewModel.EndTime = doctor.EndTime.ToString();
            doctorDetailsViewModel.Degree = doctor.Degree.ToString();
            doctorDetailsViewModel.Email = doctor.Email.ToString();
            doctorDetailsViewModel.Name = doctor.Name.ToString();
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
            if (appointmentsLst == null)
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
                if (appointment.PatientId != null)
                {
                    var patient = await _dbcontext.Patients.Find(s => s.ObjectId == appointment.PatientId).FirstOrDefaultAsync();
                    appointmentsViewModel.PatientName = patient.Name;
                    appointmentsViewModel.PatientId = patient.ObjectId;
                }
                appointmentsViewModelList.Add(appointmentsViewModel);
            }
            return View(appointmentsViewModelList);
        }

        [HttpGet]
        public async Task<IActionResult> AllAppointments()
        { 
            List<Appointment> appointmentsLst = await _dbcontext.Appointments.Find(_ => true).ToListAsync();
            if (appointmentsLst == null)
            {
                return NotFound();
            }
            List<AppointmentsViewModel> appointmentsViewModelList = new List<AppointmentsViewModel>();
            foreach (Appointment appointment in appointmentsLst)
            {
                var doctor = await _dbcontext.Doctors.Find(s => s.ObjectId == appointment.DoctorId).FirstOrDefaultAsync();
                if (doctor == null )
                {
                    return NotFound();
                }
                List<Appointment> appointmentsDLst = await _dbcontext.Appointments.Find(s => s.DoctorId == doctor.ObjectId).ToListAsync();
                var specialization = await _dbcontext.Specializations.Find(s => s.ObjectId == doctor.SpecializationId).FirstOrDefaultAsync();
                AppointmentsViewModel appointmentsViewModel = new AppointmentsViewModel();
                appointmentsViewModel.AppId = appointment.ObjectId;
                appointmentsViewModel.DoctorId = doctor.ObjectId;
                appointmentsViewModel.StartTime = doctor.StartTime.ToString();
                appointmentsViewModel.EndTime = doctor.EndTime.ToString()   ;
                appointmentsViewModel.DoctorName = doctor.Name;
                appointmentsViewModel.BookedTime = appointment.BookedTime.ToString();
                appointmentsViewModel.BookedDate = appointment.BookedDate.ToString();
                appointmentsViewModel.Specialization = specialization != null ? specialization.SpecializationName : "Unknown";
                appointmentsViewModel.Description = appointment.Description;
                if (appointment.PatientId != null)
                {
                    var patient = await _dbcontext.Patients.Find(s => s.ObjectId == appointment.PatientId).FirstOrDefaultAsync();
                    appointmentsViewModel.PatientName = patient.Name;
                    appointmentsViewModel.PatientId = patient.ObjectId;
                    appointmentsViewModel.PatientEmail = patient.Email;
                    appointmentsViewModel.PatientAge = patient.Age;
                    appointmentsViewModel.PatientPhone = patient.Phone;
                }

                appointmentsViewModelList.Add(appointmentsViewModel);
            }

            return View("DoctorAppointments", appointmentsViewModelList);

        }

        [HttpGet]
        public async Task<IActionResult> DoctorPage()
        {
            DoctorViewModel doctorViewModel = new DoctorViewModel();
            doctorViewModel.SpecializationList = await _dbcontext.Specializations.Find(_ => true).ToListAsync();
            List<Doctor> doctors = await _dbcontext.Doctors.Find(d => d.Status != "No").ToListAsync();
            doctorViewModel.DoctorInfos = new List<DoctorInfo>();
            int i = 1;
            foreach (Doctor doctor in doctors)
            {
                DoctorInfo doctorInfo = new DoctorInfo();
                doctorInfo.ObjectId = doctor.ObjectId;
                doctorInfo.Id = i++;
                doctorInfo.Name = doctor.Name;
                doctorInfo.Email = doctor.Email;
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
            return RedirectToAction("DoctorPage");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDoctor([FromBody] DoctorViewModel doctorViewModel)
        {
            var filter = Builders<Doctor>.Filter.Eq(s => s.ObjectId, doctorViewModel.UpdateDocId);
            var doctor = await _dbcontext.Doctors.Find(filter).FirstOrDefaultAsync();

            if (doctor == null)
            {
                return NotFound();
            }
            else
            {
                doctor.SpecializationId = doctorViewModel.Specialization;
                doctor.Name = doctorViewModel.Name;
                doctor.Age = doctorViewModel.Age;
                doctor.Email = doctorViewModel.Email;
                doctor.Degree = doctorViewModel.Degree;
                doctor.Description = doctorViewModel.Description;

                if (!string.IsNullOrEmpty(doctorViewModel.DoctorImage))
                {
                    doctor.DoctorImage = SaveFileFromBase64(doctorViewModel.DoctorImage, _webHostEnvironment);
                }

                if (TimeOnly.TryParse(doctorViewModel.StartTime, out TimeOnly startTime))
                    doctor.StartTime = startTime;
                if (TimeOnly.TryParse(doctorViewModel.EndTime, out TimeOnly endTime))
                    doctor.EndTime = endTime;

                var update = Builders<Doctor>.Update
                    .Set(d => d.SpecializationId, doctor.SpecializationId)
                    .Set(d => d.Name, doctor.Name)
                    .Set(d => d.Age, doctor.Age)
                    .Set(d => d.Email, doctor.Email)
                    .Set(d => d.Degree, doctor.Degree)
                    .Set(d => d.Description, doctor.Description)
                    .Set(d => d.StartTime, doctor.StartTime)
                    .Set(d => d.EndTime, doctor.EndTime);

                if (!string.IsNullOrEmpty(doctorViewModel.DoctorImage))
                {
                    update = update.Set(d => d.DoctorImage, doctor.DoctorImage);
                }
                await _dbcontext.Doctors.UpdateOneAsync(filter, update);

                return RedirectToAction("DoctorPage");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor([FromBody] DoctorViewModel doctorViewModel)
        {

            var filter = Builders<Doctor>.Filter.Eq(s => s.ObjectId, doctorViewModel.UpdateDocId);
            var doctor = await _dbcontext.Doctors.Find(filter).FirstOrDefaultAsync();
            if (doctor == null)
            {
                return NotFound();
            }
            else
            {
                var update = Builders<Doctor>.Update
                    .Set(d => d.Status, "No");

                await _dbcontext.Doctors.UpdateOneAsync(filter, update);
                return RedirectToAction("DoctorPage");
            }
        }

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
            && s.Phone == doctorDetailsViewModel.PatientPhone).FirstOrDefaultAsync();
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
            return RedirectToAction("DoctorPage");
        }
        
        private string SaveFileFromBase64(string base64String, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String) || !base64String.Contains(","))
                    return null;

                byte[] fileBytes = Convert.FromBase64String(base64String.Split(',')[1]);
                string fileName = $"{Guid.NewGuid()}.jpg";
                string directoryPath = Path.Combine(webHostEnvironment.WebRootPath, "Images", "Doctor");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, fileName);
                using (var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(fileBytes))
                {
                    var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                    {
                        Quality = 50 
                    };
                    image.Save(filePath, encoder);
                }
                return $"/Images/Doctor/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrescriptionDetails(string doctorId, string patientId , string appointmentId)
        {
            if (string.IsNullOrEmpty(doctorId) || string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Doctor ID and Patient ID are required.");
            }

            var doctor = await _dbcontext.Doctors.Find(d => d.ObjectId == doctorId).FirstOrDefaultAsync();
            Patient patient = await _dbcontext.Patients.Find(p => p.ObjectId == patientId).FirstOrDefaultAsync();
            var appointment = await _dbcontext.Appointments.Find(a => a.ObjectId == appointmentId ).FirstOrDefaultAsync();
            var specialization = await _dbcontext.Specializations.Find(s => s.ObjectId == doctor.SpecializationId).FirstOrDefaultAsync();

            var review = await _dbcontext.Reviews
    .Find(r => r.DoctorId == doctor.ObjectId && r.PatientId == patient.ObjectId && r.AppId == appointmentId)
    .FirstOrDefaultAsync();

            var viewModel = new PrescriptionDetailsViewModel
            {
                DoctorId = doctor.ObjectId,
                PatientId = patient.ObjectId,
                DoctorName = doctor.Name,
                DoctorAge = doctor.Age,
                DoctorEmail = doctor.Email,
                DoctorSpecialization = specialization?.SpecializationName ?? "Unknown",
                DoctorDegree = doctor.Degree,
                DoctorDescription = doctor.Description,
                PatientName = patient.Name,
                PatientAge = patient.Age,
                PatientEmail = patient.Email,
                PhoneNumber = patient.Phone,
                BookedTime = appointment.BookedTime.ToString(),
                BookedDate = appointment.BookedDate.ToString(),
                Description = appointment.Description,
                Prescription = appointment.Prescription,
                AppId = appointmentId,
                Rating = review?.Rating,
                Comments = review?.Comments,
                UpdatedDate = review?.UpdatedDate
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePrescription(string prescriptionText, string AppId)
        {
            if (string.IsNullOrEmpty(AppId))
            {
                return BadRequest("Appointment ID is required.");
            }

            var filter = Builders<Appointment>.Filter.Eq(a => a.ObjectId, AppId);
            var update = Builders<Appointment>.Update.Set(a => a.Prescription, prescriptionText ?? string.Empty); // Allow empty string

            var result = await _dbcontext.Appointments.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound("Appointment not found or prescription not updated.");
            }

            var appointment = await _dbcontext.Appointments.Find(a => a.ObjectId == AppId).FirstOrDefaultAsync();
            return RedirectToAction("PrescriptionDetails", new { doctorId = appointment.DoctorId, patientId = appointment.PatientId, appointmentId = appointment.ObjectId });
        }


        [HttpPost]
        public async Task<IActionResult> SubmitReview(string doctorId, string patientId, string appointmentId, int rating, string comments)
        {
            if (string.IsNullOrEmpty(appointmentId))
            {
                return BadRequest("Appointment ID is required.");
            }

            var filter = Builders<Review>.Filter.Eq(r => r.DoctorId, doctorId) &
                         Builders<Review>.Filter.Eq(r => r.PatientId, patientId) &
                         Builders<Review>.Filter.Eq(r => r.AppId, appointmentId);

            var update = Builders<Review>.Update
                .Set(r => r.Rating, rating)
                .Set(r => r.Comments, comments)
                .Set(r => r.UpdatedDate, DateTime.UtcNow);

            var options = new UpdateOptions { IsUpsert = true };

            var result = await _dbcontext.Reviews.UpdateOneAsync(filter, update, options);

            if (result.MatchedCount == 0 && result.UpsertedId == null)
            {
                return NotFound("Review not found and insertion failed.");
            }

            return RedirectToAction("PrescriptionDetails", new { doctorId = doctorId, patientId = patientId, appointmentId = appointmentId });
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(string doctorId, string date)
        {
            if (string.IsNullOrEmpty(doctorId) || string.IsNullOrEmpty(date))
            {
                return Json(new List<object>());
            }
            try
            {
                DateTime selectedDate = DateTime.Parse(date);

                var doctor = await _dbcontext.Doctors.Find(s => s.ObjectId == doctorId).FirstOrDefaultAsync();
                if (doctor == null)
                {
                    return Json(new List<object>());
                }

                var selectedDateOnly = DateOnly.FromDateTime(selectedDate);
                var bookedAppointments = await _dbcontext.Appointments
                    .Find(a => a.DoctorId == doctorId && a.BookedDate == selectedDateOnly)
                    .ToListAsync();

                var allTimeSlots = GenerateTimeSlots(doctor.StartTime, doctor.EndTime);

                var result = allTimeSlots.Select(slot => new
                {
                    id = slot.Id,
                    time = slot.Time,
                    isBooked = bookedAppointments.Any(a => {
                        if (DateTime.TryParseExact(slot.Time, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime slotTime))
                        {
                            return a.BookedTime.ToString("HH:mm") == slotTime.ToString("HH:mm");
                        }
                        return false;
                    })
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        private List<TimeSlot> GenerateTimeSlots(TimeOnly startTime, TimeOnly endTime)
        {
            var slots = new List<TimeSlot>();
            var slotDuration = TimeSpan.FromMinutes(15);
            var currentTime = startTime;
            var id = 1;

            var timeSpanEnd = endTime.ToTimeSpan();

            while (currentTime.ToTimeSpan() <= timeSpanEnd.Subtract(slotDuration))
            {
                slots.Add(new TimeSlot
                {
                    Id = id.ToString(),
                    Time = FormatTimeSlot(currentTime)
                });

                currentTime = currentTime.Add(slotDuration);
                id++;
            }

            return slots;
        }


        private string FormatTimeSlot(TimeOnly time)
        {
            return time.ToString(@"hh\:mm tt");
        }

        private class TimeSlot
        {
            public string Id { get; set; }
            public string Time { get; set; }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateAppointment([FromBody] AppointmentsViewModel appointmentsViewModel)
        {
            var filter = Builders<Appointment>.Filter.Eq(s => s.ObjectId, appointmentsViewModel.AppId);
            var appointment = await _dbcontext.Appointments.Find(filter).FirstOrDefaultAsync();

            if (appointment == null)
            {
                return NotFound();
            }

            else
            {
                if (TimeOnly.TryParse(appointmentsViewModel.BookedTime, out TimeOnly bookedTime))
                    appointment.BookedTime = bookedTime;
                if (DateOnly.TryParse(appointmentsViewModel.BookedDate, out DateOnly bookedDate))
                    appointment.BookedDate = bookedDate;
                appointment.Description = appointmentsViewModel.Description;
                var patfilter = Builders<Patient>.Filter.Eq(s => s.ObjectId, appointmentsViewModel.PatientId);
                var patient = await _dbcontext.Patients.Find(patfilter).FirstOrDefaultAsync();
                if (patient.Email == appointmentsViewModel.PatientEmail &&
                    patient.Phone == appointmentsViewModel.PatientPhone)
                {
                    //patient.Name = appointmentsViewModel.PatientName;
                    //patient.Age = appointmentsViewModel.PatientAge;
                    var patupdate = Builders<Patient>.Update
                        .Set(p => p.Name, appointmentsViewModel.PatientName)
                        .Set(p => p.Email, appointmentsViewModel.PatientEmail)
                        .Set(p => p.Phone, appointmentsViewModel.PatientPhone)
                        .Set(p => p.Age, appointmentsViewModel.PatientAge);
                  await  _dbcontext.Patients.UpdateOneAsync(patfilter, patupdate);
                }
                else
                {
                    Patient newPatient = new Patient();
                    newPatient.Email = appointmentsViewModel.PatientEmail;
                    newPatient.Age = appointmentsViewModel.PatientAge;
                    newPatient.Phone = appointmentsViewModel.PatientPhone;
                    newPatient.Name = appointmentsViewModel.PatientName;
                    await _dbcontext.Patients.InsertOneAsync(newPatient);
                    appointment.PatientId = newPatient.ObjectId;
                }

                var appupdate = Builders<Appointment>.Update
                        .Set(a => a.PatientId, appointment.PatientId)
                        .Set(a => a.BookedTime, appointment.BookedTime)
                        .Set(a => a.BookedDate, appointment.BookedDate)
                        .Set(a => a.Description, appointment.Description);
                        
               await _dbcontext.Appointments.UpdateOneAsync(filter, appupdate);

            }
            return RedirectToAction("DoctorPage");


        }


        [HttpDelete]
        public IActionResult DeleteAppointment([FromBody] AppointmentsViewModel appointmentsViewModel)
        {
            if (string.IsNullOrEmpty(appointmentsViewModel.AppId))
            {
                return BadRequest("Invalid ID");
            }

            var appointment = _dbcontext.Appointments.Find(s => s.ObjectId == appointmentsViewModel.AppId).FirstOrDefault();
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            var result = _dbcontext.Appointments.DeleteOne(s => s.ObjectId == appointmentsViewModel.AppId);

            if (result.DeletedCount == 0)
            {
                return NotFound("Appointment not found");
            }

            return Ok("Appointment deleted successfully");
        }

    }
}

