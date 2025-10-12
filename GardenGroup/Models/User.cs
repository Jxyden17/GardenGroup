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

        [BsonElement("userId")]
        public int UserId { get; set; }

        // A simple string field for the user's name.
        // Default = "" so it's never null when creating a new User.
        [BsonElement("naam")]
        public string? Name { get; set; } = "";

        [BsonElement("achternaam")]
        public string? LastName { get; set; } = "";

        [BsonElement("rol")]
        public string? Role { get; set; } = "";

        // A simple string field for the user's email.
        // Later we could add validation (e.g. DataAnnotations).
        [BsonElement("email")]
        public string? Email { get; set; } = "";

        [BsonElement("telefoonnummer")]
        public string? PhoneNumber { get; set; } = "";

        [BsonElement("stad")]
        public string? City { get; set; } = "";

        [BsonElement("password")]
        public string? Password { get; set; } = "";

        [BsonElement("salt")]
        public string? Salt { get; set; } = "";

        public User(string? id,int userId, string name, string lastName, string role, string email, string phoneNumber, string city, string password, string salt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            LastName = lastName;
            Role = role;
            Email = email;
            PhoneNumber = phoneNumber;
            City = city;
            Password = password;
            Salt = salt;
        }

        public User()
        {
        }
    }
}