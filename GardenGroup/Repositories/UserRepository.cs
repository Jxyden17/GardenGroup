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
            _users = db.GetCollection<User>("Users");
        }

        public List<User> GetAll()
        {
            List<User> users = _users.Find(FilterDefinition<User>.Empty).ToList();
            return users;
        }

        public User GetById(string id)
        {
          User user =  _users.Find(user => user.Id == id).FirstOrDefault();
            return user;
        }

        public void Delete(string id)
        {
            _users.DeleteOne(user => user.Id == id);
        }

        public void Add(User user) =>
            _users.InsertOne(user);
    }
}

