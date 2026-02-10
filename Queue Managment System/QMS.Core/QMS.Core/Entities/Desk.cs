using QMS.Core.Enums;

namespace QMS.Core.Entities
{
    public class Desk
    {
        public int Id { get; set; }
        public string DeskName { get; set; }

        public DeskType? DeskType { get; set; }

        public bool IsBusy { get; set; }

        public int? HeadTicketId { get; set; }
        public int? TailTicketId { get; set; }
        public int QueueCount { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

        public Desk()
        {
            Tickets = new List<Ticket>();
        }
    }
}
