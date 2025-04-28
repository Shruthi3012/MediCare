using MongoDB.Bson.Serialization.Attributes;

namespace MediCare.ViewModel
{
    public class UserViewModel
    {
        public string Name { get; set; }

        
        public string Email { get; set; }
        
        public string Password { get; set; }

        public string Role { get; set; }
    }
}
