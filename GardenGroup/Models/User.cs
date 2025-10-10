using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using GardenGroup.Enums;

namespace GardenGroup.Models
{
    public class User
    {
        // This will be the primary key in MongoDB.
        // [BsonId] tells MongoDB this field is the "_id".
        // [BsonRepresentation(BsonType.ObjectId)] lets us use string in C#,
        // while MongoDB still stores it as a real ObjectId internally.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // A simple string field for the user's name.
        // Default = "" so it's never null when creating a new User.
        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        
        public UserRoles  Role { get; set; } = UserRoles.normal;

        // A simple string field for the user's email.
        // Later we could add validation (e.g. DataAnnotations).
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string City { get; set; } = "";
        public string Password { get; set; } = "";
        public string Salt { get; set; } = "";

        public User(string? id, string name, string lastName, UserRoles role, string email, string phoneNumber, string city, string password, string salt)
        {
            Id = id;
            Name = name;
            LastName = lastName;
            Role = role;
            Email = email;
            PhoneNumber = phoneNumber;
            City = city;
            Password = password;
            Salt = salt;
        }
    }
}