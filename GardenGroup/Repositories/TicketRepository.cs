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
    }
}
