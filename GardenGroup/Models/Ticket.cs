using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GardenGroup.Models
{
    public class Ticket
    {

        // This will be the primary key in MongoDB.
        // [BsonId] tells MongoDB this field is the "_id".
        // [BsonRepresentation(BsonType.ObjectId)] lets us use string in C#,
        // while MongoDB still stores it as a real ObjectId internally.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }   // Uniek ticketnummer (1..100)
        DateTime Datum_open { get; set; }   // Startdatum
        DateTime Datum_close { get; set; }  // Sluitdatum
        string Status { get; set; }          // "Open" | "In Progress" | "Waiting" | "Closed"
        string Title { get; set; }           // Titel van het ticket
        string Type { get; set; }            // "Bug" | "Feature" | "Incident" | "Task"
        string Prioriteit { get; set; }      // "Laag" | "Normaal" | "Hoog" | "Kritiek"
        string Description { get; set; }     // Beschrijving
        int Creator { get; set; }         // Verwijzing naar userId (maker)
        int Solver { get; set; }          // Verwijzing naar userId (oplosser)
        DateTime Deadline { get; set; }          // Deadline
        List<TicketLog> TicketLogs = new List<TicketLog>();            // Array van log entries
        public int StepsBeforeClosed { get { return TicketLogs.Count; } set; } // Aantal stappen tot ticket werd gesloten

        public Ticket(string? id, string status, string title, string type, string prioriteit, string description, int creator, int solver, DateTime deadline)
        {
            Id = id;
            Datum_open = DateTime.Now;
            Status = status;
            Title = title;
            Type = type;
            Prioriteit = prioriteit;
            Description = description;
            Creator = creator;
            Solver = solver;
            Deadline = deadline;
            

        }
        public void AddTicketLog(TicketLog ticketLog)
        {
            TicketLogs.Add(ticketLog);
        }
    }
}
