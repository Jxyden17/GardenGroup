namespace GardenGroup.Models.viewModels
{
    public class ChartViewModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public string Color { get; set; }

        public ChartViewModel()
        {
            Title = "";
            Subtitle = "";
            Color = "#f39c12";
        }
    }
}
