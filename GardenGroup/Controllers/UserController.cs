using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GardenGroup.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository repo) => _repo = repo;

        public IActionResult Index()
        {
            var users = _repo.GetAll();
            return View(users);
        }

        [HttpPost]
        public IActionResult Create(string name, string email)
        {
            var user = new User { Name = name, Email = email };
            _repo.Add(user);
            return RedirectToAction("Index");
        }
    }

}
