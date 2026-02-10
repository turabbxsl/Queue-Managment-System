using Microsoft.EntityFrameworkCore;
using QMS.Core.Entities;
using QMS.Core.Enums;
using QMS.Core.Interfaces.Repositories;
using QMS.Infrastructure.Context;

namespace QMS.Infrastructure.Repositories
{
    public class DeskRepository : Repository<Desk>, IDeskRepository
    {

        public DeskRepository(QMSDbContext context)
       : base(context) { }


        public async Task<Desk> GetLeastBusyDeskAsync(DeskType deskType)
        {
            return await _context.Desks
                .Where(d => d.DeskType == deskType)
                .OrderBy(x => x.QueueCount)
                .ThenBy(x => x.IsBusy)
                .FirstOrDefaultAsync();
        }
    }
}
