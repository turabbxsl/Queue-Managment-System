using QMS.Core.Interfaces.Repositories;

namespace QMS.Core.Interfaces
{
    public interface IUnitOfWork
    {
        ITicketRepository Tickets { get; }
        IDeskRepository Desks { get; }
        Task<int> SaveAsync();
    }
}
