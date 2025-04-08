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

        public DoctorController(MongoDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DoctorPage()
        {
            DoctorViewModel doctorViewModel = new DoctorViewModel();
            doctorViewModel.SpecializationList = await _dbcontext.Specializations.Find(_ => true).ToListAsync();
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
    }
}
