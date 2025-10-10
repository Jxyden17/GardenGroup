using GardenGroup.Models;

namespace GardenGroup.Services.interfaces
{
    public interface IUserService
    {
        User GetUserById(string id);
       void AddUser(User user);

       void UpdateUser(User user);
        List<User> GetAllUsers();
        void DeleteUser(string id);

    }
}
