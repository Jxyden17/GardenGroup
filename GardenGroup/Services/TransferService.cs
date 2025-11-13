using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace GardenGroup.Services
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository _transferRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransferService(ITransferRepository transferRepository, UserManager<ApplicationUser> userManager)
        {
            _transferRepository = transferRepository;
            _userManager = userManager;
        }

        // ------------------------------ TransferTicketAsync ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid:
        // TransferTicketAsync is veranderwoordelijk voor het transferen van een ticket aan een andere ServiceDesk gebruiker.   
        // Ontwerpkeuzes:
        // - Ik heb gekozen om die te maken dat allen servicedesk en admin zo kunnen transfer doen aan een serviceDesk gebruiker.
        // - De methode roept de repository asynchroon aan om de applicatie responsief te houden.
        // -----------------------------------------------------------------------------
        public async Task<bool> TransferTicketAsync(string id, string newSolverId)
        {
            bool ok = await _transferRepository.TransferTicketAsync(id, newSolverId);
            return ok;
        }

        // ------------------------------ GetSecrviceDeskUsersAsync ---------------------------------------
        // Auteur: Ernest Jureko
        // Verantwoordelijkheid:
        // GetSecrviceDeskUsersAsync is veranderwoordelijk voor ophallen van alle ServiceDesk gebruikers behalve de huidige gebruiker.   
        // Ontwerpkeuzes:
        // - ik heb gekozen om die zo te doen want als ik was alle gebruikers ophalen dan zou de huidige gebruiker ook in de lijst staan wat niet nodig is,
        // maar ook als ik zo huidigegebruiker nog een keer probeeren zetten als solver kwam error dat ticket word unassigned.
        // - De methode roept de repository asynchroon aan om de applicatie responsief te houden.
        // -----------------------------------------------------------------------------
        public async Task<IList<ApplicationUser>> GetServiceDeskUsersAsync(string currentUser)
        {
            IList<ApplicationUser> allUsers = await _userManager.GetUsersInRoleAsync("ServiceDesk");
            if (allUsers == null)
            {
                return new List<ApplicationUser>();
            }
            ObjectId current = ObjectId.Parse(currentUser);
            return allUsers
                .Where(u => u.Id != current)
                .ToList();
        }
    }
}
