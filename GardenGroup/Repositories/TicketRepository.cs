using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using MongoDB.Bson;
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

        public Ticket GetTicketById(string id)
        {
            Ticket ticket = _tickets.Find(ticket => ticket.Id == id).FirstOrDefault();
            return ticket;
        }

        public void Delete(string id)
        {
            _tickets.DeleteOne(ticket => ticket.Id == id);
        }

        public void UpdateTicket(Ticket ticket)
        {
            FilterDefinition<Ticket> filter = Builders<Ticket>.Filter.Eq("_id", new ObjectId(ticket.Id));

            UpdateDefinition<Ticket> update = Builders<Ticket>.Update
                .Set(t => t.Datum_open, ticket.Datum_open)
                .Set(t => t.Datum_close, ticket.Datum_close)
                .Set(t => t.Status, ticket.Status)
                .Set(t => t.Title, ticket.Title)
                .Set(t => t.Type, ticket.Type)
                .Set(t => t.Prioriteit, ticket.Prioriteit)
                .Set(t => t.Description, ticket.Description)
                .Set(t => t.Creator, ticket.Creator)
                .Set(t => t.Solver, ticket.Solver)
                .Set(t => t.Deadline, ticket.Deadline);

            UpdateResult result = _tickets.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                throw new Exception("No records updated!");
            }
        }
    }
}
