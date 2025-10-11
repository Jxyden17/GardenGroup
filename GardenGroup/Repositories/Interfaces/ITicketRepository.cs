using GardenGroup.Models;

namespace GardenGroup.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        List<Ticket> GetAll();
        Ticket GetTicketById(string id);
        void UpdateTicket(Ticket ticket);
        void Delete(string id);
    }
}
