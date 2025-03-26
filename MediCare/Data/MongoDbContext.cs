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


    }
}
