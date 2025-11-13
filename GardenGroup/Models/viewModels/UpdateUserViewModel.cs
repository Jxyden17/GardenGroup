using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GardenGroup.Models.viewModels
{
    public class UpdateUserViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;

        public UpdateUserViewModel(string id, string firstName, string lastName, string email, string phoneNumber, string city, string password, string role)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            City = city;
            Password = password;
            Role = role;
        }

        public UpdateUserViewModel()
        {
        }
    }
}
