using AspNetCore.Identity.Mongo.Model;

namespace GardenGroup.Models
{
    public class ApplicationUser : MongoUser
    {
        // These extra fields are optional, but you can mirror your domain model.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
    }
}
