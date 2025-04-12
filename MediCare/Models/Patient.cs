using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediCare.Models
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
        
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

    }
}
