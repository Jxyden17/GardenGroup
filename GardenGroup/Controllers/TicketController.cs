using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GardenGroup.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketRepository _repo;

        public TicketController(ITicketRepository repo) => _repo = repo;
        // GET: TicketController
        public ActionResult Index()
        {
            try
            {
                List<Ticket> tickets = _repo.GetAll();
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
            Ticket ticket = _repo.GetById(id);
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

        // GET: TicketController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TicketController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: TicketController/Delete/5
        public ActionResult Delete(string id)
        {
            Ticket ticket = _repo.GetById(id);
            return View(ticket);
        }

        // POST: TicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, IFormCollection collection)
        {
            try
            {
                _repo.Delete(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
