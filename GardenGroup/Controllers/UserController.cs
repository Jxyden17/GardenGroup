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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ---------------- Index ---------------- //
        public async Task<IActionResult> Index()
        {
            try
            {
                List<ApplicationUser> users = _userManager.Users.ToList();

                var userRoles = new Dictionary<string, string>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userRoles[user.Id.ToString()] = roles.FirstOrDefault() ?? "User";
                }

                ViewBag.UserRoles = userRoles;
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving users.";
                Console.WriteLine(ex.ToString());
                return View(new List<ApplicationUser>());
            }
        }

        // ---------------- Login ---------------- //
        [AllowAnonymous]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginModel.email);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "Invalid email/password.";
                    return View(loginModel);
                }

                var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, true, false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Ticket");

                ViewBag.ErrorMessage = result.IsLockedOut
                    ? "Account locked. Try again later."
                    : "Invalid email or password.";

                return View(loginModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred during login.";
                Console.WriteLine(ex.ToString());
                return View(loginModel);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while logging out.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }

        // ---------------- Create ---------------- //
        public IActionResult Create()
        {
            try
            {
                ViewBag.AllRoles = GetAllRoles();
                return View(new CreateUserViewModel());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load create user form.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            try
            {
                ViewBag.AllRoles = GetAllRoles();

                if (await EmailExists(model.Email))
                {
                    ViewBag.ErrorMessage = "Email is already in use.";
                    return View(model);
                }

                ApplicationUser user = MapModelToUser(model);
                IdentityResult result = await CreateUserAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    ViewBag.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    return View(model);
                }

                await EnsureRoleExists(model.Role);
                await _userManager.AddToRoleAsync(user, model.Role);

                TempData["ConfirmMessage"] = $"User {user.Email} created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the user.";
                Console.WriteLine(ex.ToString());
                return View(model);
            }
        }

        // ---------------- Details ---------------- //
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectWithError("Invalid user ID.");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return RedirectWithError("User not found.");

                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRole = roles.FirstOrDefault() ?? "User";

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving user details.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }

        // ---------------- Update ---------------- //
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectWithError("Invalid user ID.");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return RedirectWithError("User not found.");

                var model = await MapUserToViewModel(user);
                ViewBag.AllRoles = GetAllRoles();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the update form.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateUserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return RedirectWithError("User not found.");

                ViewBag.AllRoles = GetAllRoles();

                IdentityResult updateResult = await UpdateUserBasicInfo(user, model);
                if (!updateResult.Succeeded)
                {
                    ViewBag.ErrorMessage = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return View(model);
                }

                var passwordResult = await UpdatePasswordIfChanged(user, model.Password);
                if (passwordResult != null && !passwordResult.Succeeded)
                {
                    ViewBag.ErrorMessage = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                    return View(model);
                }

                await UpdateUserRoleIfChanged(user, model.Role);
                TempData["ConfirmMessage"] = "User updated successfully";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while updating the user.";
                Console.WriteLine(ex.ToString());
                return View(model);
            }
        }

        // ---------------- Delete ---------------- //
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectWithError("Invalid user ID.");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return RedirectWithError("User not found.");

                var roles = await _userManager.GetRolesAsync(user);
                var model = new DeleteUserViewModel
                {
                    Id = user.Id.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    Role = roles.FirstOrDefault() ?? "User"
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading delete confirmation.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return RedirectWithError("Invalid user ID.");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return RedirectWithError("User not found.");

                IdentityResult result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return RedirectWithError(string.Join(", ", result.Errors.Select(e => e.Description)));

                TempData["ConfirmMessage"] = $"User {user.Email} deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the user.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }

        // ---------------- Private Helper Methods ---------------- //

        private IActionResult RedirectWithError(string message)
        {
            try
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to redirect with error message.", ex);
            }
        }

        private List<string> GetAllRoles()
        {
            try
            {
                return new List<string> { "User", "ServiceDesk", "Admin" };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve role list.", ex);
            }
        }

        private async Task<bool> EmailExists(string email)
        {
            try
            {
                ApplicationUser existingUser = await _userManager.FindByEmailAsync(email);
                return existingUser != null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if email '{email}' exists.", ex);
            }
        }

        private ApplicationUser MapModelToUser(CreateUserViewModel model)
        {
            try
            {
                return new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    City = model.City,
                    EmailConfirmed = true
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to map CreateUserViewModel to ApplicationUser.", ex);
            }
        }

        private async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            try
            {
                return await _userManager.CreateAsync(user, password);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create user '{user.Email}'.", ex);
            }
        }

        private async Task EnsureRoleExists(string roleName)
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    IdentityResult roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to create role '{roleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to ensure role '{roleName}' exists.", ex);
            }
        }

        private async Task<UpdateUserViewModel> MapUserToViewModel(ApplicationUser user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                return new UpdateUserViewModel
                {
                    Id = user.Id.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    Role = roles.FirstOrDefault() ?? "User"
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to map ApplicationUser '{user.Email}' to UpdateUserViewModel.", ex);
            }
        }

        private async Task<IdentityResult> UpdateUserBasicInfo(ApplicationUser user, UpdateUserViewModel model)
        {
            try
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email; // Keep UserName in sync with email
                user.PhoneNumber = model.PhoneNumber;
                user.City = model.City;

                return await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update basic info for user '{user.Email}'.", ex);
            }
        }

        private async Task<IdentityResult?> UpdatePasswordIfChanged(ApplicationUser user, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
                return null;

            try
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                return await _userManager.ResetPasswordAsync(user, token, newPassword);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update password for user '{user.Email}'.", ex);
            }
        }

        private async Task UpdateUserRoleIfChanged(ApplicationUser user, string newRole)
        {
            try
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (!currentRoles.Contains(newRole))
                {
                    IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to remove existing roles for user '{user.Email}': {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
                    }

                    IdentityResult addResult = await _userManager.AddToRoleAsync(user, newRole);
                    if (!addResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to add role '{newRole}' to user '{user.Email}': {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update roles for user '{user.Email}'.", ex);
            }
        }
    }
}

