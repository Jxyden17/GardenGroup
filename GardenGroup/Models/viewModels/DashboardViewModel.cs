// ------------------------------ DashboardViewModel ---------------------------------------
// Auteur: Ernest Jureko
// Verantwoordelijkheid: DashboardViewModel maakt alle charts voor dashboarden
//
// Ontwerpkeuzes:
// - Ik heb hier alle nodige charts toegevoegd die we op alle dashboarden willen weergeven.
// - Deze klasse maakt gebruik van ChartViewModel om de structuur van elke grafiek te definiëren.
// - Deze kunnen we later makelijk uitbreiden als we meer grafieken willen toevoegen.  
// -----------------------------------------------------------------------------
namespace GardenGroup.Models.viewModels
{
    public class DashboardViewModel
    {
        public ChartViewModel Unresolved { get; set; }
        public ChartViewModel PastDeadline { get; set; }
        public ChartViewModel ClaimedCount { get; set; }
        public ChartViewModel ClosedByMeCount { get; set; }
        public ChartViewModel TotaalTicketsOpen { get; set; }

        public DashboardViewModel()
        {
            Unresolved = new ChartViewModel();
            PastDeadline = new ChartViewModel();
            ClaimedCount = new ChartViewModel();
            ClosedByMeCount = new ChartViewModel();
            TotaalTicketsOpen = new ChartViewModel();
        }
    }
}