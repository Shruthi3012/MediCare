﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediCare.Models
{
    public class Doctor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("StartTime")]
        public TimeOnly StartTime { get; set; }
       
        [BsonElement("EndTime")]
        public TimeOnly EndTime { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string SpecializationId { get; set; }

        
        [BsonElement("Degree")]
        public string Degree { get; set; }

        
    }

    
}
