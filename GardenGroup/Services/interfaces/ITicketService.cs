using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GardenGroup.Services.interfaces
{
    public interface ITicketService
    {
        Task add(string userid, Ticket ticket);
        void UpdateTicket(Ticket ticket);
        Ticket GetTicketById(string id);
        void DeleteTicket(string id);
        List<Ticket> GetAllTickets();
        List<Ticket> GetMyTickets(string id);
        Task<DashboardViewModel> BuildForCurrentUserAsync( string id);
        Task<bool> TransferTicketAsync(string id, string newSolverUserId);
    }
}