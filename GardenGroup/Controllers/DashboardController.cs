using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GardenGroup.Controllers
{
    public class DashboardController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly ITicketService _ticketService;

        public DashboardController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IEmailService emailService, ITicketService ticketService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _ticketService = ticketService;
        }

        // /Dashboard/Employee
        [HttpGet]
        public async Task<IActionResult> Employee()
        {
            try
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                List<Ticket> myTickets = _ticketService.GetMyTickets(user.Id.ToString());
                DashboardViewModel stats = await _ticketService.BuildForCurrentUserAsync(user.Id.ToString());

                DashboardDataViewModel page = new DashboardDataViewModel
                {
                    DisplayName = !string.IsNullOrWhiteSpace(user.FirstName)
                        ? user.FirstName
                        : (!string.IsNullOrWhiteSpace(user.Email) ? user.Email : $"User {user.Id}"),
                    MyTickets = myTickets,
                    Stats = stats
                };

                return View("Employee/Index", page);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error while loading the dashboard: " + ex.Message;
                return RedirectToAction("Index", "Ticket");
            }
        }

        // /Dashboard/ServiceDesk
        [HttpGet]
        public async Task<IActionResult> ServiceDesk()
        {
            try
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                List<Ticket> claimedByMe = _ticketService.GetMySolvedTickets(user.Id.ToString());


                DashboardViewModel stats = await _ticketService.BuildForSolver(user.Id.ToString());

                DashboardDataViewModel page = new DashboardDataViewModel
                {
                    DisplayName = !string.IsNullOrWhiteSpace(user.FirstName)
                        ? user.FirstName
                        : (!string.IsNullOrWhiteSpace(user.Email) ? user.Email : $"User {user.Id}"),
                    ClaimedByMe = claimedByMe,
                    Stats = stats
                };

                return View("ServiceDesk/Index", page);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error while loading the dashboard: " + ex.Message;
                return RedirectToAction("Index", "Ticket");
            }
        }


        // /Dashboard/Admin
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            try
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                List<Ticket> allOpen = _ticketService.GetAllTickets();
                DashboardViewModel stats = await _ticketService.BuildForAdmin(user.Id.ToString());

                DashboardDataViewModel page = new DashboardDataViewModel
                {
                    DisplayName = !string.IsNullOrWhiteSpace(user.FirstName)
                        ? user.FirstName
                        : (!string.IsNullOrWhiteSpace(user.Email) ? user.Email : $"User {user.Id}"),
                    GetAllOpen = allOpen,
                    Stats = stats
                };

                return View("Admin/Index", page);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error while loading the dashboard: " + ex.Message;
                return RedirectToAction("Index", "Ticket");
            }
        }
    }
}
