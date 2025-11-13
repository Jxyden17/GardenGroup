using GardenGroup.Enums;
using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MongoDB.Driver;

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

        public List<Ticket> GetMySolvedTickets(string id)
        {
            List<Ticket> tickets = _ticketRepository.GetBySolver(id);
            return tickets;
        }

        public void GetMyClaimedAndClosedCounts(string solverId, out int claimed, out int closedByMe)
        {
            _ticketRepository.GetMyClaimedAndClosedCounts(solverId, out claimed, out closedByMe);
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

            viewModel.Unresolved.Title = "Open Ticket";
            viewModel.Unresolved.Subtitle = "All tickets that are open by You";
            viewModel.Unresolved.Value1 = counts.Unresolved;
            viewModel.Unresolved.Value2 = counts.Total;
            viewModel.Unresolved.Color = "#f39c12";

            viewModel.PastDeadline.Title = "Incidents past deadline";
            viewModel.PastDeadline.Subtitle = "These tickets are now pass DeadLine";
            viewModel.PastDeadline.Value1 = counts.PastDeadline;
            viewModel.PastDeadline.Value2 = 0;
            viewModel.PastDeadline.Color = "#c0392b";

            return viewModel;
        }

        public async Task<DashboardViewModel> BuildForSolver(string solverId)
        {
            _ticketRepository.GetMyClaimedAndClosedCounts(solverId, out int claimed, out int closedByMe);

            DashboardViewModel viewModel = new DashboardViewModel();

            viewModel.ClaimedCount.Title = "Tickets claimed by You";
            viewModel.ClaimedCount.Subtitle = "Tickets you are currently working on";
            viewModel.ClaimedCount.Value1 = claimed;
            viewModel.ClaimedCount.Value2 = 0;
            viewModel.ClaimedCount.Color = "#2980b9";

            viewModel.ClosedByMeCount.Title = "Tickets closed by You";
            viewModel.ClosedByMeCount.Subtitle = "Tickets you have successfully closed";
            viewModel.ClosedByMeCount.Value1 = closedByMe;
            viewModel.ClosedByMeCount.Value2 = 0; 
            viewModel.ClosedByMeCount.Color = "#27ae60";

            return viewModel;
        }

        public async Task<DashboardViewModel> BuildForAdmin(string adminId)
        {
            List<Ticket> allTicekts = _ticketRepository.GetAll();

            int totalTickets = allTicekts.Count;

            DashboardViewModel viewModel = new DashboardViewModel();
            
            viewModel.TotaalTicketsOpen.Title = "Total Tickets";
            viewModel.TotaalTicketsOpen.Subtitle = "All tickets that are currently open in the system";
            viewModel.TotaalTicketsOpen.Value1 = totalTickets;
            viewModel.TotaalTicketsOpen.Value2 = totalTickets;
            viewModel.TotaalTicketsOpen.Color = "#8e44ad";
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
