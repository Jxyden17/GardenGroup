using GardenGroup.Enums;

namespace GardenGroup.Models
{
    public class TicketLog
    {
       int Step { get; set; }        // Volgnummer van de stap
      TicketLogType SctionType { get; set; }  // "CREATED" | "STATUS_CHANGED" | "COMMENTED"
       string StatusFrom { get; set; }  // (optioneel, bij statuswissel)
       string StatusTo { get; set; }   // (optioneel, bij statuswissel)
       string Message { get; set; }     // (optioneel, bij comment)
       DateTime At { get; set; }           // Tijdstip van de actie
       User Actor { get; set; } 
        // Snapshot van de user die actie deed


    }
}
