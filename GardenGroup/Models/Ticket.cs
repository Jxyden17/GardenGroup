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
        [BsonElement("ticketId")]
        public int TicketId { get; set; }
        [BsonElement("datum_open")]
        public DateTime Datum_open { get; set; }   // Startdatum
        [BsonElement("datum_close")]
        public DateTime? Datum_close { get; set; }  // Sluitdatum
        [BsonElement("status")]
        public string Status { get; set; }          // "Open" | "In Progress" | "Waiting" | "Closed"
        [BsonElement("title")]
        public string Title { get; set; }           // Titel van het ticket
        [BsonElement("type")]
        public string Type { get; set; }            // "Bug" | "Feature" | "Incident" | "Task"
        [BsonElement("prioriteit")]
        public string Prioriteit { get; set; }      // "Laag" | "Normaal" | "Hoog" | "Kritiek"
        [BsonElement("description")]
        public string Description { get; set; }     // Beschrijving
        [BsonElement("creator")]
        public int Creator { get; set; }         // Verwijzing naar userId (maker)
        [BsonElement("solver")]
        public int Solver { get; set; }          // Verwijzing naar userId (oplosser)
        [BsonElement("deadline")]
        public DateTime Deadline { get; set; }          // Deadline
        public List<TicketLog> TicketLogs = new List<TicketLog>();            // Array van log entries
        public int StepsBeforeClosed { get { return TicketLogs.Count; } } // Aantal stappen tot ticket werd gesloten

        public Ticket(string? id, int ticketId, string status, string title, string type, string prioriteit, string description, int creator, int solver, DateTime deadline)
        {
            Id = id;
            TicketId = ticketId;
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
