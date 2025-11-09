using System.Security.Claims;
using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GardenGroup.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserController(IUserService userService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            try
            {
                List<ApplicationUser> users = _userManager.Users.ToList();

                return View(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = "Fout bij data van users ophalen, probeer later.";
                return View(new List<ApplicationUser>());
            }
        }

        [AllowAnonymous]
        public IActionResult Login() => View();


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid form submission.";
                return View(loginModel);
            }

            // Find user in Identity system
            ApplicationUser user = await _userManager.FindByEmailAsync(loginModel.email);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid email/password.";
                return View(loginModel);
            }

            // Attempt sign-in
            var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Optionally: load domain user info (not required for Identity)
                return RedirectToAction("Index", "Ticket");
            }

            if (result.IsLockedOut)
            {
                ViewBag.ErrorMessage = "Account locked. Try again later.";
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
            }

            return View(loginModel);
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Form validation failed.";
                return View(model);
            }

            // Check if email already exists
            ApplicationUser existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email is already in use.";
                return View(model);
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                City = model.City,
                DisplayRole = model.Role,
                EmailConfirmed = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = model.Role });
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["SuccessMessage"] = $"User {user.Email} created successfully!";
            return RedirectToAction("Index");
        }


        public IActionResult Details(string id)
        {
            User user = _userService.GetUserById(id);
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.ErrorMessage = "Invalid user ID.";
                return RedirectToAction("Index");
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return RedirectToAction("Index");
            }


            var roles = await _userManager.GetRolesAsync(user);
            UpdateUserViewModel model = new UpdateUserViewModel
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                Role = roles.FirstOrDefault() ?? "User" // Default role
            };

            ViewBag.AllRoles = new List<string> { "User", "ServiceDesk", "Admin" }; // For radio buttons
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateUserViewModel model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return RedirectToAction("Index");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email; // Keep UserName in sync with email
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ViewBag.AllRoles = new List<string> { "User", "ServiceDesk", "Admin" };
                ViewBag.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            // Update password if changed
            if (!string.IsNullOrEmpty(model.Password))
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!passResult.Succeeded)
                {
                    ViewBag.AllRoles = new List<string> { "User", "ServiceDesk", "Admin" };
                    ViewBag.ErrorMessage = string.Join(", ", passResult.Errors.Select(e => e.Description));
                    return View(model);
                }
            }

            // Update role if changed
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            TempData["ConfirmMessage"] = "User updated successfully";
            return RedirectToAction("Index");
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
    }

}
