using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;

namespace GardenGroup.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public void add(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Ticket GetTicketById(string ticketId)
        {
            return _ticketRepository.GetTicketById(ticketId);
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
           return  _ticketRepository.GetAll();
        }
    }
}
