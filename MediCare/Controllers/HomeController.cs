using MediCare.Data;
using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Diagnostics;

namespace MediCare.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

       
        private readonly MongoDbContext _context;

        public HomeController(MongoDbContext context)
        {
            
            _context = context;
           
        }


        public async Task<IActionResult> TestMongoConnection()
        {
            try
            {
                // Attempt to list all collections in the database to check the connection
                var collections = await _context.GetDatabase().ListCollectionNamesAsync();
                var collectionList = await collections.ToListAsync();

                // If we get a list of collections, the connection is successful
                return Content($"MongoDB Connection Successful! Collections: {string.Join(", ", collectionList)}");
            }
            catch (Exception ex)
            {
                return Content($"MongoDB Connection Failed: {ex.Message}");
            }
        }




        public async Task<IActionResult> Index()
        {
            var doctorNames = new List<string>();
            var appointmentsPerDoctor = new List<int>();
            var avgRatings = new List<double>();

            var doctors = await _context.Doctors.Find(_ => true).ToListAsync();

            foreach (var doctor in doctors)
            {
                doctorNames.Add(doctor.Name);

                var appointments = await _context.Appointments.Find(a => a.DoctorId == doctor.ObjectId).ToListAsync();
                appointmentsPerDoctor.Add(appointments.Count);

                var reviews = await _context.Reviews.Find(r => r.DoctorId == doctor.ObjectId).ToListAsync();
                avgRatings.Add(reviews.Any() ? reviews.Average(r => r.Rating) : 0);
            }

            var prescriptions = await _context.Prescriptions.Find(_ => true).ToListAsync();
            var prescriptionsGrouped = prescriptions
                .GroupBy(p => p.PrescriptionDate.Date)
                .Select(g => new { Date = g.Key.ToShortDateString(), Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToList();

            ViewBag.DoctorNames = doctorNames;
            ViewBag.AppointmentsPerDoctor = appointmentsPerDoctor;
            ViewBag.AvgRatings = avgRatings;
            ViewBag.PrescriptionDates = prescriptionsGrouped.Select(x => x.Date).ToList();
            ViewBag.PrescriptionCounts = prescriptionsGrouped.Select(x => x.Count).ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
