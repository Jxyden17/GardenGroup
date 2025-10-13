using GardenGroup.Models;

namespace GardenGroup.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        void Add(User user);
        User GetUserById(string id);
        void Delete(string id);
        // Future:
        void UpdateUser(User user);
        User GetUserByEmail(string email);
    }
}

