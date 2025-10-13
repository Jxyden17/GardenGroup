using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GardenGroup.Repositories
{

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly IPasswordService _passwordService;

        public UserRepository(IMongoDatabase db, IPasswordService passwordService)
        {
            _users = db.GetCollection<User>("Users");
            _passwordService = passwordService;
        }

        public List<User> GetAll()
        {
            List<User> users = _users.Find(FilterDefinition<User>.Empty).ToList();
            return users;
        }

        public User GetUserById(string id)
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

        public void UpdateUser(User user)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("_id", new ObjectId(user.Id));

            UpdateDefinition<User> update = Builders<User>.Update
                .Set(u => u.Name, user.Name)
                .Set(u => u.LastName, user.LastName)
                .Set(u => u.Role, user.Role)
                .Set(u => u.Email, user.Email)
                .Set(u => u.PhoneNumber, user.PhoneNumber)
                .Set(u => u.City, user.City)
                .Set(u => u.Password, user.Password)
                .Set(u => u.Salt, user.Salt);

            UpdateResult result = _users.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                throw new Exception("No records updated!");
            }
        }

        public User GetUserByEmail(string email)
        {
            User user = _users.Find(user => user.Email == email ).FirstOrDefault();

            return user;

            
        }
    }
}
