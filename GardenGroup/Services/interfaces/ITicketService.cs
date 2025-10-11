using GardenGroup.Models;

namespace GardenGroup.Services.interfaces
{
    public interface ITicketService
    {
        void add(Ticket ticket);
        void UpdateTicket(Ticket ticket);
        Ticket GetTicketById(string ticketId);
    }
}
