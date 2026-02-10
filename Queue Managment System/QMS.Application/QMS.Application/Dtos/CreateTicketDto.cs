using QMS.Core.Enums;

namespace QMS.Application.Dtos
{
    public class CreateTicketDto
    {
        public string CustomerFullName { get; set; }
        public CustomerType CustomerType { get; set; }
        public ServiceType ServiceType { get; set; }
        public DeskType DeskType { get; set; }
    }
}
