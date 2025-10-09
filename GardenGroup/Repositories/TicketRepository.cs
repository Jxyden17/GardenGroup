using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using MongoDB.Driver;

namespace GardenGroup.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;
        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("Tickets");
        }
        public void Add(Ticket ticket)
        {
           _tickets.InsertOne(ticket);
        }

        public List<Ticket> GetAll()
        {
            List<Ticket> tickets = _tickets.Find(FilterDefinition<Ticket>.Empty).ToList();
            return tickets;
        }

        public Ticket GetById(string id)
        {
            Ticket ticket = _tickets.Find(ticket => ticket.Id == id).FirstOrDefault();
            return ticket;
        }

        public void Delete(string id)
        {
            _tickets.DeleteOne(ticket => ticket.Id == id);
        }
    }
}
