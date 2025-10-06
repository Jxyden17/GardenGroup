using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using MongoDB.Driver;

namespace GardenGroup.Repositories
{

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("users");
        }

        public List<User> GetAll() =>
            _users.Find(_ => true).ToList();

        public void Add(User user) =>
            _users.InsertOne(user);
    }
}

