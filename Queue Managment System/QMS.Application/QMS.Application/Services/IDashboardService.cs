using QMS.Application.Dtos;
using QMS.Core.Enums;
using QMS.Core.Interfaces;

namespace QMS.Application.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetBankLiveStatusAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;

        public DashboardService(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public async Task<DashboardDto> GetBankLiveStatusAsync()
        {
            var desks = (await _uow.Desks.GetAllAsync()).OrderBy(d => d.DeskName).ToList();
            var allTickets = await _uow.Tickets.GetAllAsync();

            var dashboard = new DashboardDto()
            {
                LastUpdated = DateTime.Now,
                TotalWaiting = desks.Sum(x => x.QueueCount),
                Desks = new List<DeskStatusDto>()
            };

            foreach (var desk in desks)
            {
                var deskDto = new DeskStatusDto()
                {
                    DeskId = desk.Id,
                    DeskName = desk.DeskName,
                    Status = desk.IsBusy ? "Məşğul" : "Boş",
                    QueueCount = desk.QueueCount,
                    NextThreeCustomers = new List<NextCustomerDto>()
                };

                var activeTicket = allTickets.FirstOrDefault(t => t.DeskId == desk.Id && t.Status == TicketStatus.InProgress);
                deskDto.CurrentCustomer = activeTicket?.TicketNumber ?? "Yoxdur";
                deskDto.CurrentCustomerName = activeTicket?.CustomerFullName ?? "---";

                int? nextId = desk.HeadTicketId;
                int count = 0;

                while (nextId != null && count < 3)
                {
                    var ticket = allTickets.FirstOrDefault(x => x.Id == nextId);

                    if (ticket != null)
                    {
                        deskDto.NextThreeCustomers.Add(new NextCustomerDto
                        {
                            TicketNumber = ticket.TicketNumber,
                            CustomerName = ticket.CustomerFullName ?? "Adsız Müştəri"
                        });

                        nextId = ticket.NextTicketId;
                    }
                    else break;

                    count++;
                }

                dashboard.Desks.Add(deskDto);
            }

            return dashboard;
        }
    }
}
