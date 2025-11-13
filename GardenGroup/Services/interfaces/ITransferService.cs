using GardenGroup.Models;

namespace GardenGroup.Services.interfaces
{
    public interface ITransferService
    {
        Task<bool> TransferTicketAsync(string id, string newSolverId);
        Task<IList<ApplicationUser>> GetServiceDeskUsersAsync(string currentUser);
    }
}
