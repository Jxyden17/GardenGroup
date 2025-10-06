using GardenGroup.Models;

namespace GardenGroup.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        void Add(User user);

        // Future:
        // User? GetById(string id);
        // void Update(User user);
        // void Delete(string id);
    }
}

