using QMS.Core.Interfaces;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Context;

namespace QMS.Infrastructure.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly QMSDbContext _context;

        public ITicketRepository Tickets { get; }
        public IDeskRepository Desks { get; }

        public UnitOfWork(QMSDbContext context)
        {
            _context = context;
            Tickets = new TicketRepository(context);
            Desks = new DeskRepository(context);
        }

        public async Task<int> SaveAsync()
            => await _context.SaveChangesAsync();
    }
}
