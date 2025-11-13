using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using MongoDB.Driver;

namespace GardenGroup.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;
        public TransferRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("Tickets");
        }

        // ------------------------------ TransferTicketAsync ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid: TransferTicketAsync is een methode die zorg dat ticket kan doorgegeven aan andere service desk gebruiker
        //
        // Ontwerpkeuzes:
        // - Allen solver veld woord aanpest bij transferen van ticket.
        // - De methode returnt bool om aan te geven of de transfer succesvol was.
        // - Ik heb gekozen om UpdateOneAsync te vorkomen dat de server zo vastlopen.
        // -----------------------------------------------------------------------------

        public async Task<bool> TransferTicketAsync(string ticketid, string newSolverId)
        {

            FilterDefinition<Ticket> filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticketid);
            UpdateDefinition<Ticket> update = Builders<Ticket>.Update.Set(t => t.Solver, newSolverId);

            UpdateResult result = await _tickets.UpdateOneAsync(filter, update);

            return result.MatchedCount == 1 && result.ModifiedCount == 1;
        }
    }
}
