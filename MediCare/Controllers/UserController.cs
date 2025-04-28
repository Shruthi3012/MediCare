using MediCare.Data;
using MediCare.Models;
using MediCare.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Diagnostics;

namespace MediCare.Controllers
{

    public class UserController : Controller
    {

        private readonly MongoDbContext _dbcontext;
        public UserController(MongoDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult UserReg()
        {
            //var user = new User
            //{
            //    Name = "test",
            //    Email = "test@gmail.com",
            //    Password  = "password",
            //    Role = "Patient"
            //};

            //_dbcontext.Users.InsertOne(user);
            return View();
        }

        [HttpPost]
        public IActionResult Adduser([FromBody] UserViewModel userViewModel)
        {
            User newUser = new User();
            newUser.Name = userViewModel.Name;
            newUser.Email = userViewModel.Email;
            newUser.Password = userViewModel.Password;
            newUser.Role = userViewModel.Role;

            _dbcontext.Users.InsertOne(newUser);
            return View("UserReg");
        }


    }
}
