using AspNetCore.Identity.Mongo.Model;

namespace GardenGroup.Models
{
    public class ApplicationRole : MongoRole
    {
        // Optional: add extra fields if needed
        public string? Description { get; set; }
    }
}