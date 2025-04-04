﻿using MediCare.Data;
using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MediCare.Controllers
{
    public class SpecializationController : Controller
    {
        private readonly MongoDbContext _dbcontext;

        public SpecializationController(MongoDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SpecPage()
        {
            var specializationsList = await _dbcontext.Specializations.Find(s => true).ToListAsync();
            return View(specializationsList);
        }

        [HttpPost]
        public IActionResult AddSpec([FromBody] Specialization specialization)
        {
            _dbcontext.Specializations.InsertOne(specialization);
            return RedirectToAction("SpecPage");
        }

        [HttpDelete]
        public IActionResult DeleteSpec(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID");
            }

            var specialization = _dbcontext.Specializations.Find(s => s.ObjectId == id).FirstOrDefault(); 
            if (specialization == null)
            {
                return NotFound("Specialization not found");
            }

            var result = _dbcontext.Specializations.DeleteOne(s => s.ObjectId == id);

            if (result.DeletedCount == 0)
            {
                return NotFound("Specialization not found");
            }

            return Ok("Specialization deleted successfully");
        }

    }
}
