using GardenGroup.Models;
using GardenGroup.Models.viewModels;
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
            List<Ticket> tickets = _tickets
                .Find(tickets => tickets.Status != Enums.TicketStatuses.Closed)
                .Limit(150)
                .ToList();
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
                .Set(t => t.Deadline, ticket.Deadline);

            UpdateResult result = _tickets.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                throw new Exception("No records updated!");
            }
        }

        public List<Ticket> GetByCreator(string creatorId)
        {
            List<Ticket> tickets = _tickets
                .Find(ticket => ticket.Creator == creatorId && ticket.Status != Enums.TicketStatuses.Closed)
                .Limit(100)
                .ToList();
            return tickets;
        }

        public DashboardCountsViewModel GetDashboardCountsForUser(string creatorId)
        {
            DateTime nowUtc = DateTime.UtcNow;
            List<BsonDocument> pipeline = new List<BsonDocument>();

            // stap 1 - filter: enkel tickets van de gebruiker
            BsonDocument match = new BsonDocument("$match",
                new BsonDocument("creator", creatorId)
            );

            // stap 2 - groeperen en tellen
            BsonArray unresolvedCond = new BsonArray
            {
                new BsonDocument("$ne", new BsonArray { "$status", "Closed" }),
                1,
                0
            };

            BsonArray pastDeadlineCond = new BsonArray
            {
                new BsonDocument("$and", new BsonArray
                {
                    new BsonDocument("$ne", new BsonArray { "$status", "Closed" }),
                    new BsonDocument("$lt", new BsonArray { "$deadline", nowUtc })
                }),
                1,
                0
            };

            BsonDocument group = new BsonDocument("$group",
                new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "total", new BsonDocument("$sum", 1) },
                    { "unresolved", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray(unresolvedCond))) },
                    { "pastDeadline", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray(pastDeadlineCond))) }
                }
            );

            pipeline.Add(match);
            pipeline.Add(group);

            // uitvoeren
            IAsyncCursor<BsonDocument> cursor = _tickets.Aggregate<BsonDocument>(pipeline);
            BsonDocument result = cursor.FirstOrDefault();

            DashboardCountsViewModel counts = new DashboardCountsViewModel();
            if (result != null)
            {
                counts.Total = result.GetValue("total", 0).ToInt32();
                counts.Unresolved = result.GetValue("unresolved", 0).ToInt32();
                counts.PastDeadline = result.GetValue("pastDeadline", 0).ToInt32();
            }

            return counts;
        }

        public bool TransferTicket(string ticketid, string newSolverUserId)
        {

            FilterDefinition<Ticket> filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticketid);
            UpdateDefinition<Ticket> update = Builders<Ticket>.Update.Set(t => t.Solver, newSolverUserId);

            UpdateResult result = _tickets.UpdateOne(filter, update);

            return result.MatchedCount == 1 && result.ModifiedCount == 1;
        }
    }
}