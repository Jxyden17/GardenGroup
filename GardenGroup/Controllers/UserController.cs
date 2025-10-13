using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GardenGroup.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            try
            {
                List<User> users = _userService.GetAllUsers();
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
            User user = _userService.GetUserById(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult Update(User user)
        {
            try
            {
                _userService.UpdateUser(user);
                TempData["ConfirmMessage"] = "Your user has been edited succesfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occured: {ex.Message}";
                return View(user);
            }
        }

        [HttpGet]
        public ActionResult Update(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User user = _userService.GetUserById(id);
            return View(user);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            User user = _userService.GetUserById(id);
            return View(user);
        }

        [HttpPost,]
        public IActionResult DeleteConfirmed(string id)
        {
            _userService.DeleteUser(id);
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
