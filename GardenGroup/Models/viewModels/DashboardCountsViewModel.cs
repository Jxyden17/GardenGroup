namespace GardenGroup.Models.viewModels
{
    public class DashboardCountsViewModel
    {
        public int Total { get; set; }
        public int Unresolved { get; set; }
        public int PastDeadline { get; set; }

        public int ClaimedCount { get; set; }
        public int ClosedByMeCount { get; set; }

        public int TotaalTicketsOpen { get; set; }
    }
}