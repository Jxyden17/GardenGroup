using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GardenGroup.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IArchiveService _archiveService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(ITicketService ticketService, UserManager<ApplicationUser> userManager, IArchiveService archiveService)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _archiveService = archiveService;

        }

        [Authorize(Roles = "Admin,ServiceDesk")]
        // GET: TicketController
        public ActionResult Index()
        {
            try
            {
                List<Ticket> tickets = _ticketService.GetAllTickets();
                return View(tickets);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = "Fout bij data van tickets ophalen probeer later.";
                return View(new List<Ticket>());
            }
        }

        [Authorize(Roles = "Admin,ServiceDesk")]
        // GET: TicketController/Details/5
        public ActionResult Details(string id)
        {
            Ticket ticket = _ticketService.GetTicketById(id);
            return View("Details", ticket);
        }

        [Authorize(Roles = "Admin,ServiceDesk")]
        // GET: TicketController/Create
        public ActionResult Create()
        {
            Ticket ticket = new Ticket();
            return View("Create", ticket);
        }

        // POST: TicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ServiceDesk")]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            try
            {
                await _ticketService.add(user.Id.ToString(), ticket);
                TempData["SuccessMessage"] = "Ticket created successfully!";
                return RedirectToAction("Employee", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to create ticket: " + ex.Message;
                return View("Create", ticket);
            }
        }

        [Authorize(Roles = "Admin,ServiceDesk")]
        [HttpPost]
        public ActionResult Update(Ticket ticket)
        {
            try
            {
                _ticketService.UpdateTicket(ticket);
                TempData["ConfirmMessage"] = "Your ticket has been edited successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View(ticket);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ServiceDesk")]
        public ActionResult Update(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = _ticketService.GetTicketById(id);
            return View(ticket);
        }


        // GET: TicketController/Delete/5
        [Authorize(Roles = "Admin,ServiceDesk")]
        public ActionResult Delete(string id)

        {
            Ticket ticket = _ticketService.GetTicketById(id);
            return View("Delete", ticket);
        }

        // POST: TicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ServiceDesk")]
        public ActionResult DeleteConfirmed(string id, IFormCollection collection)
        {
            try
            {
                _ticketService.DeleteTicket(id);
                TempData["SuccessMessage"] = "Ticket deleted successfully!";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete ticket: " + ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin,ServiceDesk")]
        public ActionResult Archive()
        {
            try
            {
                List<Ticket> tickets = _ticketService.GetAllTickets();
                _archiveService.Archive(tickets);
                return View();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "archiveren mislukt";
                return View();
            }

        }


        [Authorize(Roles = "Admin,ServiceDesk")]
        [HttpGet]
        public IActionResult Transfer(string id)
        {
            Ticket ticket = _ticketService.GetTicketById(id);
            var serviceDeskUsers =  _userManager.GetUsersInRoleAsync("ServiceDesk").Result;
            ViewBag.Users = serviceDeskUsers;
            return View(ticket);
        }

        [Authorize(Roles = "Admin,ServiceDesk")]
        [HttpPost]
        public async Task<IActionResult> Transfer(string id, string newSolverUserId)
        {
            try
            {
                List<Ticket> tickets = _ticketService.GetAllTickets();
                _archiveService.Archive(tickets);
                return View();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "archiveren mislukt";
                return View();
            }
            
        }
    }
}
