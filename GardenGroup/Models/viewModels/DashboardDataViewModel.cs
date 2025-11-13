// ------------------------------ DashboardDataViewModel ---------------------------------------
// Auteur: Ernest Jureko
// Verantwoordelijkheid: DashboardDataViewModel is een viewmodel dat alle gegevens voor het dashboard vertegenwoordigt.
//
// Ontwerpkeuzes:
// - ik heb gekozen om hier lijsten van tickets op te nemen voor verschillende secties en rolen van het dashboarden.
// - ik heb ook gekozen om DashboardViewModel op te nemen voor statistieken om alles georganiseerd te houden.
// - Deze word gebruikt in DashboardController om alle nodige data naar de view te sturen.  
// -----------------------------------------------------------------------------
namespace GardenGroup.Models.viewModels
{
    public class DashboardDataViewModel
    {
        public string DisplayName { get; set; }
        public List<Ticket> ClaimedByMe { get; set; }
        public List<Ticket> MyTickets { get; set; }
        public List<Ticket> GetAllOpen { get; set; }
        public DashboardViewModel Stats { get; set; }

        public DashboardDataViewModel()
        {
            DisplayName = "";
            ClaimedByMe = new List<Ticket>();
            Stats = new DashboardViewModel();
            MyTickets = new List<Ticket>();
            GetAllOpen = new List<Ticket>();
        }
    }
}