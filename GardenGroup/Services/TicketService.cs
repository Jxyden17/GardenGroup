using GardenGroup.Enums;
using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GardenGroup.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketService(ITicketRepository ticketRepository, UserManager<ApplicationUser> userManager)
        {
            _ticketRepository = ticketRepository;
            _userManager = userManager;
        }

        public async Task add(string userid, Ticket ticket)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userid);
            ticket.Datum_open = DateTime.Now;
            ticket.Status = TicketStatuses.Open;
            ticket.Creator = user.Id.ToString();
            ticket.Solver = string.Empty;

            _ticketRepository.Add(ticket);
        }

        public Ticket GetTicketById(string id)
        {
            Ticket ticket = _ticketRepository.GetTicketById(id);
    if (ticket == null) return null; // make return type Ticket? if your interface allows

    List<ApplicationUser> users = _userManager.Users.ToList();

    ApplicationUser? creator = users.FirstOrDefault(u => u.Id.ToString() == ticket.Creator);
    ApplicationUser? solver  = users.FirstOrDefault(u => u.Id.ToString() == ticket.Solver);

    ticket.CreatorName = creator?.FirstName ?? creator?.Email ?? "Unknown";
    ticket.SolverName  = solver?.FirstName  ?? solver?.Email  ?? "Unassigned";
            return ticket;

        }

        public void UpdateTicket(Ticket ticket)
        {
            _ticketRepository.UpdateTicket(ticket);
        }

        public void DeleteTicket(string id)
        {
            _ticketRepository.Delete(id);
        }

        public List<Ticket> GetAllTickets()
        {
            return _ticketRepository.GetAll();
        }

        public List<Ticket> GetMyTickets(string id)
        {
            List<Ticket> tickets = _ticketRepository.GetByCreator(id);
            return tickets;
        }

        public async Task<DashboardCountsViewModel> GetCountsForCurrentUserAsync(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new DashboardCountsViewModel();
            }

            DashboardCountsViewModel counts = _ticketRepository.GetDashboardCountsForUser(id);
            return counts;
        }

        public async Task<DashboardViewModel> BuildForCurrentUserAsync(string id)
        {
            DashboardCountsViewModel counts = await GetCountsForCurrentUserAsync(id);

            DashboardViewModel viewModel = new DashboardViewModel();

            viewModel.Unresolved.Title = "Unresolved incidents";
            viewModel.Unresolved.Subtitle = "All tickets currently open";
            viewModel.Unresolved.Value1 = counts.Unresolved;
            viewModel.Unresolved.Value2 = counts.Total;
            viewModel.Unresolved.Color = "#f39c12";

            viewModel.PastDeadline.Title = "Incidents past deadline";
            viewModel.PastDeadline.Subtitle = "These tickets need your immediate attention";
            viewModel.PastDeadline.Value1 = counts.PastDeadline;
            viewModel.PastDeadline.Value2 = 0;
            viewModel.PastDeadline.Color = "#c0392b";

            return viewModel;
        }

        public async Task<bool> TransferTicketAsync(string id, string newSolverUserId)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(newSolverUserId))
                return false;

            bool ok = _ticketRepository.TransferTicket(id, newSolverUserId);
            return ok;
        }
    }
}
