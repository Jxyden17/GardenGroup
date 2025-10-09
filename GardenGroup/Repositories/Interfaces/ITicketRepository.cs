using GardenGroup.Models;

namespace GardenGroup.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        List<Ticket> GetAll();
        Ticket GetById(string id);
        void Delete(string id);
    }
}
