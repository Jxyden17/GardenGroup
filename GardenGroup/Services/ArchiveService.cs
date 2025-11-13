using GardenGroup.Models;
using GardenGroup.Repositories.Interfaces;
using GardenGroup.Services.interfaces;
using System.Collections.Generic;

namespace GardenGroup.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly IArchiveRepository _archiveRepository;

        public ArchiveService(IArchiveRepository archiveRepository)
        {
            _archiveRepository = archiveRepository;
        }
        
        public void Archive(List<Ticket> tickets)
        {
            List < Ticket > closedTickets =_archiveRepository.GetClosedTickets(tickets);
            Archiver archiver=new Archiver(closedTickets);
            _archiveRepository.Archive(archiver);
        }
    }
}
