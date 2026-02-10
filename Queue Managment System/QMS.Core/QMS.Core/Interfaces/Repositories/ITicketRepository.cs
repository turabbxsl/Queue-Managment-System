using QMS.Core.Entities;

namespace QMS.Core.Interfaces.Repositories
{
    public interface ITicketRepository :IRepository<Ticket>
    {
        Task<Ticket> GetActiveTicketByDeskAsync(int deskId);
        Task<List<Ticket>> GetAllWaitingTicketsByDeskAsync(int deskId);
    }
}
