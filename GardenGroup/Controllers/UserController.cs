using System.Security.Claims;
using GardenGroup.Models;
using GardenGroup.Models.Extensions;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Login loginModel)
        {
            User? user =  _userService.GetUserByLoginCredentials(loginModel.email, loginModel.Password);
            try
            {
                if(user == null)
                {
                    ViewBag.ErrorMessage = "Invalid email or password.";
                    return View(loginModel);
                }
                else
                {
                    //Create user claims
                    List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("UserId", user.Id),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    AuthenticationProperties authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
                    return RedirectToAction("Index", "Ticket");
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View(loginModel);
            }

        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");
            return RedirectToAction("Login", "User");
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
