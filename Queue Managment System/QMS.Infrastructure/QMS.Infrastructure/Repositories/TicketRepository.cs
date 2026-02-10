using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Enums;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Context;

namespace QMS.Infrastructure.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(QMSDbContext context)
        : base(context) { }

        public async Task<IEnumerable<Ticket>> GetWaitingTicketsAsync()
        {
            return await _context.Tickets
                .Where(x => x.Status == TicketStatus.Waiting)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Ticket> GetNextTicketForDeskAsync(int deskId)
        {
            return await _context.Tickets
                .Where(x => x.DeskId == deskId
                         && x.Status == TicketStatus.Waiting)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Ticket> GetActiveTicketByDeskAsync(int deskId)
        {
           return await _context.Tickets
             .FirstOrDefaultAsync(t => t.DeskId == deskId && t.Status == TicketStatus.InProgress);
        }

        public async Task<List<Ticket>> GetAllWaitingTicketsByDeskAsync(int deskId)
        {
            return await _context.Tickets
            .Where(t => t.DeskId == deskId && t.Status == TicketStatus.Waiting).ToListAsync();
        }
    }
}
