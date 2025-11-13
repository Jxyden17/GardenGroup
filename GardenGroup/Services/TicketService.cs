using GardenGroup.Enums;
using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MongoDB.Driver;
using MongoDB.Bson;

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

            if (ticket == null) return null;

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

        public async Task<DashboardCountsViewModel> GetCountsForCurrentUserAsync(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            
            DashboardCountsViewModel counts = await _ticketRepository.GetDashboardUserAsync(id);
            return counts;
        }
        // ------------------------------ BuildForCurrentUserAsync ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid:
        // BuildForCurrentUserAsync is veranderwoordelijk voor het samenstellen van een dashboardweergave voor de huidige Employee.   
        // BuildForSolver is veranderwoordelijk voor het samenstellen van een dashboardweergave voor de huidige ServiceDesk.
        // BuildForAdmin is veranderwoordelijk voor het samenstellen van een dashboardweergave voor de Admin.
        // Ontwerpkeuzes:
        // - ik heb gekozen om de dashboard data op te bouwen in de service laag zodat controller schoon blijft.
        // - Alle data die de charts nodig hebben komt uit het counts object dat uit de repository komt. 
        // -----------------------------------------------------------------------------
        public async Task<DashboardViewModel> BuildForCurrentUserAsync(string id)
        {
            DashboardCountsViewModel counts = await GetCountsForCurrentUserAsync(id);

            DashboardViewModel viewModel = new DashboardViewModel();

            viewModel.Unresolved.Title = "Open Ticket";
            viewModel.Unresolved.Subtitle = "All tickets that are open by You";
            viewModel.Unresolved.Value1 = counts.Unresolved;
            viewModel.Unresolved.Value2 = counts.Total;
            viewModel.Unresolved.Color = "#f39c12";

            viewModel.PastDeadline.Title = "Tickets past deadline";
            viewModel.PastDeadline.Subtitle = "These tickets are now pass DeadLine";
            viewModel.PastDeadline.Value1 = counts.PastDeadline;
            viewModel.PastDeadline.Value2 = counts.Total;
            viewModel.PastDeadline.Color = "#c0392b";

            return viewModel;
        }

        public async Task<DashboardViewModel> BuildForSolverAsync(string solverId)
        {
            DashboardCountsViewModel counts = await _ticketRepository.GetSolverDashboardAsync(solverId);

            DashboardViewModel viewModel = new DashboardViewModel();

            viewModel.ClaimedCount.Title = "Tickets claimed by You";
            viewModel.ClaimedCount.Subtitle = "Tickets you are currently working on";
            viewModel.ClaimedCount.Value1 = counts.ClaimedCount;
            viewModel.ClaimedCount.Value2 = counts.Total;
            viewModel.ClaimedCount.Color = "#2980b9";

            viewModel.ClosedByMeCount.Title = "Tickets closed by You";
            viewModel.ClosedByMeCount.Subtitle = "Tickets you have successfully closed";
            viewModel.ClosedByMeCount.Value1 = counts.ClosedByMeCount;
            viewModel.ClosedByMeCount.Value2 = counts.Total; 
            viewModel.ClosedByMeCount.Color = "#27ae60";

            return viewModel;
        }

        public async Task<DashboardViewModel> BuildForAdminAsync()
        {
            DashboardCountsViewModel counts = await _ticketRepository.GetAdminDashboardAsync();

            DashboardViewModel viewModel = new DashboardViewModel();
            
            viewModel.TotaalTicketsOpen.Title = "Total open Tickets";
            viewModel.TotaalTicketsOpen.Subtitle = "All open ticket is systeem";
            viewModel.TotaalTicketsOpen.Value1 = counts.TotaalTicketsOpen;
            viewModel.TotaalTicketsOpen.Value2 = counts.Total;
            viewModel.TotaalTicketsOpen.Color = "#8e44ad";

            viewModel.ClosedByMeCount.Title = "Ticket closed";
            viewModel.ClosedByMeCount.Subtitle = "All tickets that are closed today";
            viewModel.ClosedByMeCount.Value1 = counts.ClosedToday;
            viewModel.ClosedByMeCount.Value2 = counts.Total;
            viewModel.ClosedByMeCount.Color = "#8e44ad";

            viewModel.PastDeadline.Title = "Tickets over Deadline";
            viewModel.PastDeadline.Subtitle = "All tickets that are currently over deadline";
            viewModel.PastDeadline.Value1 = counts.PastDeadline;
            viewModel.PastDeadline.Value2 = counts.Total;
            viewModel.PastDeadline.Color = "#8e44ad";
            return viewModel;
        }
    }
}
