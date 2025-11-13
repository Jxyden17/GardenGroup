using GardenGroup.Enums;
using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace GardenGroup.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IArchiveService _archiveService;
        private readonly ITransferService  _transferService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(ITicketService ticketService, UserManager<ApplicationUser> userManager, IArchiveService archiveService, ITransferService transferService)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _archiveService = archiveService;
            _transferService = transferService;

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

        [Authorize(Roles = "Admin,ServiceDesk,User")]
        // GET: TicketController/Details/5
        public ActionResult Details(string id)
        {
            Ticket ticket = _ticketService.GetTicketById(id);
            return View("Details", ticket);
        }

        [Authorize(Roles = "Admin,ServiceDesk,User")]
        // GET: TicketController/Create
        public ActionResult Create()
        {
            Ticket ticket = new Ticket();
            return View("Create", ticket);
        }

        // POST: TicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ServiceDesk,User")]
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

        // ------------------------------ Transfer ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid: Deze methode maak zorg dat jij kan tickets doorgeven aan andere service desk gebruiker 
        // Allen admin en servicedesk kunnen dat doen.
        //
        // Ontwerpkeuzes:
        // - Ticket id(TicketId) en newSolverId(userId) zijn nodig om ticket te vinden en nieuwe gebruiker toe te wijzen.
        // - Try catch word gebruik om fouten ophallen en weergeven aan user ook dat website ga niet crashen.
        // - Ik gebruik bool om te checken of transfer gelukt is of niet en geef feedback aan user via TempData.
        // -----------------------------------------------------------------------------
        [Authorize(Roles = "Admin,ServiceDesk")]
        [HttpGet]
        public async Task<IActionResult> Transfer(string id)
        {
            try
            {
                Ticket ticket = _ticketService.GetTicketById(id);

                ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
                IList<ApplicationUser> users = await _transferService.GetServiceDeskUsersAsync(currentUser.Id.ToString());

                TransferTicketViewModel transfer = new TransferTicketViewModel 
                {
                    Ticket = ticket,
                    ServiceDeskUsers = users
                };
                return View(transfer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Kon ticket of gebruikers niet ophalen. Probeer later opnieuw.";
                return View();
            }
        }

        [Authorize(Roles = "Admin,ServiceDesk")]
        [HttpPost]
        public async Task<IActionResult> Transfer(string id, string newSolverId)
        {
            try
            {
                bool ok = await _transferService.TransferTicketAsync(id, newSolverId);
                if (ok)
                {
                    TempData["SuccessMessage"] = "Ticket transferred.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Transfer failed (ticket not found or not modified).";
                }

                if (User.IsInRole("ServiceDesk"))
                    return RedirectToAction("ServiceDesk", "Dashboard");

                if (User.IsInRole("Admin"))
                    return RedirectToAction("Admin", "Dashboard");

                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Transfer failed (ticket not found or not modified).";
                return View();
            }
            
        }
    }
}
