using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediCare.Models
{
    public class Specialization
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }

        [BsonElement("specializationName")]
        public string SpecializationName { get; set; }

    }
}
