namespace QMS.Application.Dtos
{
    public class DeskStatusDto
    {
        public int DeskId { get; set; }
        public string DeskName { get; set; }
        public string Status { get; set; }
        public int QueueCount { get; set; }
        public string CurrentCustomer { get; set; }
        public string CurrentCustomerName { get; set; }
        public List<NextCustomerDto> NextThreeCustomers { get; set; }

        public DeskStatusDto()
        {
            NextThreeCustomers = new();
        }
    }

    public class NextCustomerDto
    {
        public string TicketNumber { get; set; }
        public string CustomerName { get; set; }
    }
}
