using MongoDB.Driver;
using MediCare.Models;

namespace MediCare.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration config)
        {
            var mongoClient = new MongoClient(config["MongoDB:ConnectionString"]);
            _database = mongoClient.GetDatabase(config["MongoDB:DatabaseName"]);
        }
        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Specialization> Specializations => _database.GetCollection<Specialization>("Specializations");

        public IMongoCollection<Doctor> Doctors => _database.GetCollection<Doctor>("Doctors");
        public IMongoCollection<Appointment> Appointments => _database.GetCollection<Appointment>("Appointments");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("Reviews");
        public IMongoCollection<Patient> Patients => _database.GetCollection<Patient>("Patients");
        public IMongoCollection<Prescription> Prescriptions => _database.GetCollection<Prescription>("Prescriptions");

    }
}
