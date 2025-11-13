using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;

namespace GardenGroup.Models
{
    public class ApplicationUser : MongoUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }

    }
}
