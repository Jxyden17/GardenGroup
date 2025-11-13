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
