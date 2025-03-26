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




        public IActionResult Index()
        {
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
