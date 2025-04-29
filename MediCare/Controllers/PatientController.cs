using MediCare.Data;
using MediCare.Models;
using MediCare.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediCare.Controllers
{
    public class PatientController : Controller
    {
        private readonly MongoDbContext _dbcontext;

        public PatientController(MongoDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetails()
        {
            var patients = await _dbcontext.Patients.Find(p => !p.IsDeleted).ToListAsync();

            var patientListViewModel = new List<PatientDetailsViewModel>();

            foreach (var patient in patients)
            {
                patientListViewModel.Add(new PatientDetailsViewModel
                {
                    PatientId = patient.ObjectId,
                    Name = patient.Name,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    Age = patient.Age
                });
            }

            return View(patientListViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(string id)
        {
            var patient = await _dbcontext.Patients.Find(p => p.ObjectId == id).FirstOrDefaultAsync();
            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new PatientDetailsViewModel
            {
                PatientId = patient.ObjectId,
                Name = patient.Name,
                Email = patient.Email,
                Phone = patient.Phone,
                Age = patient.Age
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPatient(PatientDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var filter = Builders<Patient>.Filter.Eq(p => p.ObjectId, model.PatientId);
                var update = Builders<Patient>.Update
                    .Set(p => p.Name, model.Name)
                    .Set(p => p.Email, model.Email)
                    .Set(p => p.Phone, model.Phone)
                    .Set(p => p.Age, model.Age);

                await _dbcontext.Patients.UpdateOneAsync(filter, update);
                return RedirectToAction(nameof(PatientDetails));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePatient(string id)
        {
            var filter = Builders<Patient>.Filter.Eq(p => p.ObjectId, id);
            var patient = await _dbcontext.Patients.Find(filter).FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound();
            }

            var update = Builders<Patient>.Update
                .Set(p => p.IsDeleted, true);

            await _dbcontext.Patients.UpdateOneAsync(filter, update);

            return RedirectToAction("PatientDetails");
        }


    }
}
