namespace GardenGroup.Models.viewModels
{
    public class DashboardPageViewModel
    {
        public string DisplayName { get; set; }
        public List<Ticket> MyTickets { get; set; }
        public DashboardViewModel Stats { get; set; }

        public DashboardPageViewModel()
        {
            DisplayName = "";
            MyTickets = new List<Ticket>();
            Stats = new DashboardViewModel();
        }
    }
}