namespace GardenGroup.Models
{
    public class Archiver
    {
        public List<Ticket> Tickets { get; set; }

        public Archiver(List<Ticket> tickets)
        {
            Tickets = tickets;
        }
    }
}
