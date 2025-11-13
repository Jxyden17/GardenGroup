// ------------------------------ TransferTicketViewModel ---------------------------------------
// Auteur: Ernest Jureko
// Verantwoordelijkheid: TransferTicketViewModel heb alle data nodig om ticket transfer te maken
//
// Ontwerpkeuzes:
// - Ik neem Ticket object om alle ticket details te hebben die we willen transfereren.
// - dan pak ik lijst van service desk gebruikers om te kunnen kiezen aan wie we ticket willen doorgeven.
// - Deze viewmodel maakt het makelijker om alle nodige data in de view te hebben voor transfer functionaliteit.
// -----------------------------------------------------------------------------
using GardenGroup.Models;
namespace GardenGroup.Models.viewModels
{
    public class TransferTicketViewModel
    {
        public Ticket Ticket { get; set; }
        public IList<ApplicationUser> ServiceDeskUsers { get; set; }
    }
}