using GardenGroup.Models;
using GardenGroup.Enums;
using GardenGroup.Models.viewModels;
using GardenGroup.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;

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
        // ------------------------------ GetByCreator ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid: GetByCreator is een methode die alle ticket ophaald voor creator en die zijn nog niet closed
        //
        // Ontwerpkeuzes:
        // - Ik heb hier voor gekozen dat ik ga limit zetten om niet te veel ticket ophallen tegelijk
        // - Tickets worden gefilterd op basis van creatorId en status niet closed.
        // -----------------------------------------------------------------------------

        public List<Ticket> GetByCreator(string creatorId)
        {
            List<Ticket> tickets = _tickets
                .Find(ticket => ticket.Creator == creatorId && ticket.Status != Enums.TicketStatuses.Closed)
                .Limit(100)
                .ToList();
            return tickets;
        }
        // ------------------------------ GetBySolver ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid: GetBySolver is een methode die alle ticket ophaald voor solver en die zijn nog niet closed
        //
        // Ontwerpkeuzes:
        // - Net als bij GetByCreator, ik heb gier voor gekozen dat ik ga limit zetten om niet te veel ticket ophallen tegelijk
        //  en tickets worden gefilterd op basis van solverId en status niet closed.
        // -----------------------------------------------------------------------------

        public List<Ticket> GetBySolver(string solverId)
        {
            List<Ticket> tickets = _tickets
                .Find(ticket => ticket.Solver == solverId && ticket.Status != Enums.TicketStatuses.Closed)
                .Limit(100)
                .ToList();
            return tickets;
        }
        // ------------------------------ GetDashboardUserAsync ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid: GetDashboardUserAsync ophaald de ticket counts voor dashboard van employee
        // allen tickets van creator worden geteld en geclassificeerd in total, unresolved en past deadline.
        // Ontwerpkeuzes:
        // - Aggregatie pipeline word gebruikt om data efficient te verwerken binnen MongoDB.
        // - Enum waarden worden vergeleken als strings om compatibiliteit met MongoDB te waarborgen of typefouten te voorkomen.
        // - nowUtc word gebruikt om deadlines correct te vergelijken met huidige tijd.
        // -----------------------------------------------------------------------------

        public async Task<DashboardCountsViewModel> GetDashboardUserAsync(string creatorId)
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
                    { "unresolved", new BsonDocument("$sum", new BsonDocument("$cond", unresolvedCond)) },
                    { "pastDeadline", new BsonDocument("$sum", new BsonDocument("$cond", pastDeadlineCond)) }
                }
            );

            pipeline.Add(match);
            pipeline.Add(group);

            // uitvoeren
            IAsyncCursor<BsonDocument> cursor = await _tickets.AggregateAsync<BsonDocument>(pipeline);
            BsonDocument result = await cursor.FirstOrDefaultAsync();

            DashboardCountsViewModel counts = new DashboardCountsViewModel();
            if (result != null)
            {
                counts.Total = result.GetValue("total", 0).ToInt32();
                counts.Unresolved = result.GetValue("unresolved", 0).ToInt32();
                counts.PastDeadline = result.GetValue("pastDeadline", 0).ToInt32();
            }

            return counts;
        }

        // ------------------------------ GetSolverDashboardAsync ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid: GetSolverDashboardAsync haalt de statysticen op voor het dashboard van een Service Desk medewerker (solver).
        //
        // Ontwerpkeuzes:
        // - Aggregation pipeline word gebruik om in een keer alle tellers op te hallen 
        // - Data word direct naar DashboardCountsViewModel gemapt voor eenvoudiger gebruik in de service en controller lagen.
        // - De filter selecteert enkel tickets toegewezen aan de opgegeven solverId.   
        // -----------------------------------------------------------------------------
        public async Task<DashboardCountsViewModel> GetSolverDashboardAsync(string solverId)
        {
            List<BsonDocument> pipeline = new List<BsonDocument>();

            BsonDocument match = new BsonDocument("$match",
                new BsonDocument("solver", solverId));

            BsonDocument isClosed = new BsonDocument("$eq",
               new BsonArray { "$status", "Closed" });

            BsonDocument isNotClosed = new BsonDocument("$ne", 
                new BsonArray { "$status", "Closed" });

            BsonDocument group = new BsonDocument("$group",
                new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "total", new BsonDocument("$sum", 1) },
                    { "claimed", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray { isNotClosed, 1, 0 })) },
                    { "closedByMe", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray { isClosed, 1, 0 })) }
                }
            );

            pipeline.Add(match);
            pipeline.Add(group);

            // uitvoeren
            IAsyncCursor<BsonDocument> cursor = await _tickets.AggregateAsync<BsonDocument>(pipeline);
            BsonDocument result = await cursor.FirstOrDefaultAsync();

            DashboardCountsViewModel counts = new DashboardCountsViewModel();
            if (result != null)
            {
                counts.Total = result.GetValue("total", 0).ToInt32();
                counts.ClaimedCount = result.GetValue("claimed", 0).ToInt32();
                counts.ClosedByMeCount = result.GetValue("closedByMe", 0).ToInt32();
            }

            return counts;
        }


        // ------------------------------ GetAdminDashboardAsync ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid: GetAdminDashboardAsync is gemakt om alle nodige statistieken op te halen voor het dashboard van een administrator.
        //
        // Ontwerpkeuzes:
        // - Hier pak ik counters voor totaal aantal tickets, open tickets, tickets voorbij deadline en tickets gesloten vandaag.
        // - Het is gebruik van aggregatie pipeline om de data efficiënt te verwerken binnen MongoDB en om niet 4 keer iets aan
        // - te vragen wat de performance zo verkleinen.
        // -----------------------------------------------------------------------------
        public async Task<DashboardCountsViewModel> GetAdminDashboardAsync()
        {
            DateTime nowUtc = DateTime.UtcNow;
            DateTime Today = DateTime.UtcNow.Date;

            List<BsonDocument> pipeline = new List<BsonDocument>();

            BsonArray openCond = new BsonArray
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

            BsonArray closedToday = new BsonArray
            {
                new BsonDocument("$and", new BsonArray
                {
                    new BsonDocument("$eq", new BsonArray { "$status", "Closed" }),
                    new BsonDocument("$gte", new BsonArray { "$datum_close", Today }),
                }),
                1,
                0
            };

            BsonDocument group = new BsonDocument("$group",
                new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "total", new BsonDocument("$sum", 1) },
                    { "totalopen", new BsonDocument("$sum", new BsonDocument("$cond", openCond)) },
                    { "pastDeadline", new BsonDocument("$sum", new BsonDocument("$cond", pastDeadlineCond)) },
                    { "closedToday", new BsonDocument("$sum", new BsonDocument("$cond", closedToday)) }
                }
            );

            pipeline.Add(group);

            IAsyncCursor<BsonDocument> cursor = await _tickets.AggregateAsync<BsonDocument>(pipeline);
            BsonDocument result = await cursor.FirstOrDefaultAsync();

            DashboardCountsViewModel counts = new DashboardCountsViewModel();

            if (result != null)
            {
                counts.Total = result.GetValue("total", 0).ToInt32();
                counts.TotaalTicketsOpen = result.GetValue("totalopen", 0).ToInt32();
                counts.PastDeadline = result.GetValue("pastDeadline", 0).ToInt32();
                counts.ClosedToday = result.GetValue("closedToday", 0).ToInt32();
            }
            return counts;
        }
    }
}