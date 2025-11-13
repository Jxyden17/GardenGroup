using GardenGroup.Models;
namespace GardenGroup.Repositories.Interfaces
{
    public interface IArchiveRepository
    {
        void Archive(Archiver archiver);
        List<Ticket> GetClosedTickets(List<Ticket> tickets);


    }
}
