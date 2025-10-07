using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using System.Data;
using System.Xml.Linq;

namespace GardenGroup.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        public UserService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }
        //public User GetEmployeeById(string id)
        //{
        //    return _userRepository.GetById(id);
        //}
        public void AddUser(User user1) 
        {
            string salt= _passwordService.GenerateSalt();
            string password=_passwordService.InterleaveSalt(user1.Password, salt);
            string hashedPassword=_passwordService.HashPassword(password);
            User user2= new User(user1.Id, user1.Name, user1.LastName, user1.Role, user1.Email, user1.PhoneNumber,user1.City, hashedPassword,salt);
            _userRepository.Add(user2);
        }
    }
}
