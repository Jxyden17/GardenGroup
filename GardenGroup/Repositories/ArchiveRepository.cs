using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Enums;
using MongoDB.Driver;
namespace GardenGroup.Repositories
{
    public class ArchiveRepository : IArchiveRepository
    {
        private readonly IMongoCollection<Ticket> _archive;
        private readonly IMongoCollection<Ticket> _tickets;
     
        public ArchiveRepository(IMongoDatabase db)
        {
            _archive = db.GetCollection<Ticket>("Archive");
            _tickets = db.GetCollection<Ticket>("Tickets");
        }

        public void Archive(Archiver archiver)
        {
            if (archiver.Tickets.Count >= 1)
            {
                _archive.InsertMany(archiver.Tickets);
                foreach (var ticket in archiver.Tickets)
                {
                    string id = ticket.Id;
                    _tickets.DeleteOne(ticket => ticket.Id == id);
                }
            }

        }
        public List< Ticket> GetClosedTickets(List<Ticket> tickets)
        {
            List<Ticket> closedTickets = new List<Ticket>();
            foreach (Ticket ticket in tickets)
            {
                if (ticket.Status.Equals(TicketStatuses.Closed))
                {
                    string id = ticket.Id;


                    _tickets.Find(ticket => ticket.Id == id).FirstOrDefault();
                     closedTickets.Add(ticket);
                }
            }
            return closedTickets;
        }
    }
}
