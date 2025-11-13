using GardenGroup.Models;
using GardenGroup.Models.viewModels;
using System.Threading.Tasks;

namespace GardenGroup.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        List<Ticket> GetAll();
        Ticket GetTicketById(string id);
        void UpdateTicket(Ticket ticket);
        void Delete(string id);
        List<Ticket> GetByCreator(string creatorId);
        List<Ticket> GetBySolver(string solverId);
        Task<DashboardCountsViewModel> GetDashboardUserAsync(string creatorId);
        Task<DashboardCountsViewModel> GetSolverDashboardAsync(string solverId);
        Task<DashboardCountsViewModel> GetAdminDashboardAsync();
    }
}