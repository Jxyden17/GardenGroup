namespace GardenGroup.Repositories.Interfaces
{
    public interface ITransferRepository
    {
        Task<bool> TransferTicketAsync(string ticketid, string newSolverId);
    }
}
