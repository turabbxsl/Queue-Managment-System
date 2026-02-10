using QMS.Core.Entities;
using QMS.Core.Enums;

namespace QMS.Core.Interfaces.Repositories
{
    public interface IDeskRepository:IRepository<Desk>
    {
        Task<Desk> GetLeastBusyDeskAsync(DeskType serviceType);
    }
}
