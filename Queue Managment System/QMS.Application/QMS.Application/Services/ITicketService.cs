using Microsoft.AspNetCore.SignalR;
using QMS.Application.Dtos;
using QMS.Core.Entities;
using QMS.Core.Enums;
using QMS.Core.Interfaces;
using QMS.UI.Hubs;

namespace QMS.Application.Services
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicketAsync(CreateTicketDto dto);
        Task<bool> CancelTicketAsync(int ticketId);

        Task<bool> TransferTicketAsync(int ticketId, int targetDeskId);
    }

    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHubContext<QueueHub> _hubContext;

        public TicketService(IUnitOfWork uow, IHubContext<QueueHub> hubContext)
        {
            _uow = uow;
            _hubContext = hubContext;
        }

        public async Task<bool> CancelTicketAsync(int ticketId)
        {
            var ticket = await _uow.Tickets.GetByIdAsync(ticketId);
            if (ticket == null || ticket.Status != TicketStatus.Waiting) return false;

            var desk = await _uow.Desks.GetByIdAsync(ticket.DeskId);

            if (ticket.PreviousTicketId != null) // If the deleted ticket is in the middle: Connect the one in front to the one in back
            {
                var previousTicket = await _uow.Tickets.GetByIdAsync(ticket.PreviousTicketId.Value);
                previousTicket.NextTicketId = ticket.NextTicketId;
            }
            else desk.HeadTicketId = ticket.NextTicketId;

            if (ticket.NextTicketId != null)
            {
                var nextTicket = await _uow.Tickets.GetByIdAsync(ticket.NextTicketId.Value);
                nextTicket.PreviousTicketId = ticket.PreviousTicketId;
            }
            else desk.TailTicketId = ticket.PreviousTicketId;

            ticket.Status = TicketStatus.Cancelled;
            ticket.NextTicketId = null;
            ticket.PreviousTicketId = null;

            desk.QueueCount--;

            await _uow.SaveAsync();

            await _hubContext.Clients.All.SendAsync("UpdateDashboard");

            return true;
        }

        public async Task<Ticket> CreateTicketAsync(CreateTicketDto dto)
        {
            var desk = await _uow.Desks.GetLeastBusyDeskAsync(dto.DeskType);

            var ticket = new Ticket
            {
                CustomerFullName = dto.CustomerFullName,
                ServiceType = dto.ServiceType,
                CustomerType = dto.CustomerType,
                DeskId = desk.Id,
                Status = TicketStatus.Waiting,
                TicketNumber = $"T-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}"
            };

            await _uow.Tickets.AddAsync(ticket);
            await _uow.SaveAsync();

            if (desk.HeadTicketId == null)
            {
                desk.HeadTicketId = ticket.Id;
                desk.TailTicketId = ticket.Id;
            }
            else if (ticket.CustomerType == CustomerType.VIP)
            {
                await HandleVipPlacement(desk, ticket);
            }
            else
            {
                var currentTail = await _uow.Tickets.GetByIdAsync(desk.TailTicketId.Value);

                currentTail.NextTicketId = ticket.Id;
                ticket.PreviousTicketId = currentTail.Id;

                desk.TailTicketId = ticket.Id;
            }

            desk.IsBusy = true;
            desk.QueueCount++;

            await _uow.SaveAsync();

            await _hubContext.Clients.All.SendAsync("UpdateDashboard");

            return ticket;
        }

        public async Task<bool> TransferTicketAsync(int ticketId, int targetDeskId)
        {
            var ticket = await _uow.Tickets.GetByIdAsync(ticketId);

            var currentDesk = await _uow.Desks.GetByIdAsync(ticket.DeskId);
            var targetDesk = await _uow.Desks.GetByIdAsync(targetDeskId);

            if (ticket == null || currentDesk == null || targetDesk == null) return false;

            if (ticket.PreviousTicketId != null)
            {
                var prev = await _uow.Tickets.GetByIdAsync(ticket.PreviousTicketId.Value);
                prev.NextTicketId = ticket.NextTicketId;
            }
            else currentDesk.HeadTicketId = ticket.NextTicketId;

            if (ticket.NextTicketId != null)
            {
                var next = await _uow.Tickets.GetByIdAsync(ticket.NextTicketId.Value);
                next.PreviousTicketId = ticket.PreviousTicketId;
            }
            else currentDesk.TailTicketId = ticket.PreviousTicketId;

            currentDesk.QueueCount--;

            ticket.DeskId = targetDesk.Id;
            ticket.Status = TicketStatus.Waiting;
            ticket.NextTicketId = null;
            ticket.PreviousTicketId = null;

            if (targetDesk.HeadTicketId == null)
            {
                targetDesk.HeadTicketId = ticket.Id;
                targetDesk.TailTicketId = ticket.Id;
            }
            else if (ticket.CustomerType == CustomerType.VIP)
            {
                await HandleVipPlacement(targetDesk, ticket);
            }
            else
            {
                var currentTail = await _uow.Tickets.GetByIdAsync(targetDesk.TailTicketId.Value);

                currentTail.NextTicketId = ticket.Id;
                ticket.PreviousTicketId = currentTail.Id;
                targetDesk.TailTicketId = ticket.Id;
            }

            targetDesk.QueueCount++;

            await _uow.SaveAsync();

            await _hubContext.Clients.All.SendAsync("UpdateDashboard");

            return true;
        }

        private async Task HandleVipPlacement(Desk desk, Ticket vipTicked)
        {

            var head = await _uow.Tickets.GetByIdAsync(desk.HeadTicketId.Value);

            if (head.CustomerType != CustomerType.VIP)
            {
                vipTicked.NextTicketId = head.Id;
                head.PreviousTicketId = vipTicked.Id;
                desk.HeadTicketId = vipTicked.Id;
            }
            else
            {
                var temp = head;

                while (temp != null)
                {
                    var next = await _uow.Tickets.GetByIdAsync(temp.NextTicketId.Value);
                    if (next.CustomerType != CustomerType.VIP) break;

                    temp = next;
                }

                vipTicked.NextTicketId = temp.NextTicketId;
                vipTicked.PreviousTicketId = temp.Id;

                if (temp.NextTicketId != null)
                {
                    var afterVip = await _uow.Tickets.GetByIdAsync(temp.NextTicketId.Value);
                    afterVip.PreviousTicketId = vipTicked.Id;
                }

                temp.NextTicketId = vipTicked.Id;
            }


        }
    }
}
