using GardenGroup.Models;
namespace GardenGroup.Services.interfaces
{
    public interface IArchiveService
    {
        void Archive(List<Ticket> tickets);
    }
}
