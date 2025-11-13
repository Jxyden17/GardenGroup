// ------------------------------ ChartViewModel ---------------------------------------
// Auteur: Ernest Jureko
// Verantwoordelijkheid: ChartViewModel is een viewmodel dat de gegevens voor een grafiek op het dashboard vertegenwoordigt.
//
// Ontwerpkeuzes:
// - Ik heb gekozen om die te maken zo dat ik hoef niet die elke keer te schrijfen zelf
// - De value properties zijn generiek genoeg om verschillende soorten grafieken te ondersteunen.
// -----------------------------------------------------------------------------

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
            Color = "";
        }
    }
}