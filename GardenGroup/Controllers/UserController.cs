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
            try
            {
                List<User> users = _repo.GetAll();
                return View(users);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = "Fout bij data van users ophalen probeer later.";
                return View(new List<User>());
            }
        }

        public IActionResult Details(string id)
        {
            User user = _repo.GetById(id);
            return View(user);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            User user = _repo.GetById(id);
            return View(user);
        }

        [HttpPost,]
        public IActionResult DeleteConfirmed(string id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");

        }

        //[HttpPost]
        //public IActionResult Create(string name, string email)
        //{
        //    var user = new User { Name = name, Email = email };
        //    _repo.Add(user);
        //    return RedirectToAction("Index");
        //}
    }

}
