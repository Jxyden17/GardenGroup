using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GardenGroup.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
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

        // GET: TicketController/Details/5
        public ActionResult Details(string id)
        {
            Ticket ticket = _ticketService.GetTicketById(id);
            return View(ticket);
        }

        // GET: TicketController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    
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
        public ActionResult Delete(string id)
        {
            Ticket ticket = _ticketService.GetTicketById(id);
            return View(ticket);
        }

        // POST: TicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, IFormCollection collection)
        {
            try
            {
                _ticketService.DeleteTicket(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
