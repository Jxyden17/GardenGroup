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
