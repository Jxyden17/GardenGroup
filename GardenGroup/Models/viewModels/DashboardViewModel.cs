namespace GardenGroup.Models.viewModels
{
    public class DashboardViewModel
    {
        public ChartViewModel Unresolved { get; set; }
        public ChartViewModel PastDeadline { get; set; }

        public DashboardViewModel()
        {
            Unresolved = new ChartViewModel();
            PastDeadline = new ChartViewModel();
        }
    }
}
