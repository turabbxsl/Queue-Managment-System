using QMS.Core.Enums;

namespace QMS.Core.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string CustomerFullName { get; set; }

        public ServiceType ServiceType { get; set; }
        public CustomerType CustomerType { get; set; }
        public TicketStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int DeskId { get; set; }
        public Desk Desk { get; set; }

        public int? NextTicketId { get; set; }
        public Ticket NextTicket { get; set; }

        public int? PreviousTicketId { get; set; }
        public Ticket PreviousTicket { get; set; }

    }
}
