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

    }
}
