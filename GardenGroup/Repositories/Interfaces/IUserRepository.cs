using GardenGroup.Models;

namespace GardenGroup.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        void Add(User user);
        User GetById(string id);
        void Delete(string id);
        // Future:
        // void Update(User user);
    }
}

