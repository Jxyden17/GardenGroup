// ------------------------------ DashboardCountsViewModel ---------------------------------------
// Auteur: Ernest Jureko
// Verantwoordelijkheid: DashboardCountsViewModel is een viewmodel dat de tellingen voor verschillende ticketstatistieken op het dashboard vertegenwoordigt.
//
// Ontwerpkeuzes:
// - Ik heb hier alle nodige tellingen toegevoegd die we op het dashboard willen weergeven.
// - Later word die gebruikt in DashboardDataViewModel om alle statistieken te groeperen.
// -----------------------------------------------------------------------------
namespace GardenGroup.Models.viewModels
{
    public class DashboardCountsViewModel
    {
        // Regular user
        public int Total { get; set; }
        public int Unresolved { get; set; }
        public int PastDeadline { get; set; }

        // Service desk
        public int ClaimedCount { get; set; }
        public int ClosedByMeCount { get; set; }
        //Admin 
        public int TotaalTicketsOpen { get; set; }
        public int ClosedToday { get; set; }

    }
}