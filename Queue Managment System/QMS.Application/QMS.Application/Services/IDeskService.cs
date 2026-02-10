using Microsoft.AspNetCore.SignalR;
using QMS.Core.Entities;
using QMS.Core.Enums;
using QMS.Core.Interfaces;
using QMS.UI.Hubs;

namespace QMS.Application.Services
{
    public interface IDeskService
    {
        Task<Ticket> CallNext(int deskId);
        Task FinishCurrentWork(int deskId);
    }

    public class DeskService : IDeskService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHubContext<QueueHub> _hubContext;


        public DeskService(IUnitOfWork uow, IHubContext<QueueHub> hubContext)
        {
            _uow = uow;
            _hubContext = hubContext;
        }

        public async Task<Ticket> CallNext(int deskId)
        {
            var desk = await _uow.Desks.GetByIdAsync(deskId);
            if (desk == null) return null;

            var activeTicket = await _uow.Tickets.GetActiveTicketByDeskAsync(deskId);
            if (activeTicket != null)
            {
                activeTicket.Status = TicketStatus.Completed;
            }

            if (desk.HeadTicketId == null)
            {
                desk.TailTicketId = null;
                desk.IsBusy = false;
                desk.QueueCount = 0;
                await _uow.SaveAsync();

                await _hubContext.Clients.All.SendAsync("UpdateDashboard");

                return null;
            }

            var ticketToCall = await _uow.Tickets.GetByIdAsync(desk.HeadTicketId.Value);

            desk.HeadTicketId = ticketToCall.NextTicketId;

            if (desk.HeadTicketId == null)
            {
                desk.TailTicketId = null;
            }
            else
            {
                var nextInLine = await _uow.Tickets.GetByIdAsync(desk.HeadTicketId.Value);
                if (nextInLine != null)
                {
                    nextInLine.PreviousTicketId = null;
                }
            }

            ticketToCall.Status = TicketStatus.InProgress;

            ticketToCall.NextTicketId = null;
            ticketToCall.PreviousTicketId = null;

            desk.IsBusy = true;
            if (desk.QueueCount > 0) desk.QueueCount--;

            await _uow.SaveAsync();
            await _hubContext.Clients.All.SendAsync("UpdateDashboard");

            return ticketToCall;
        }

        public async Task FinishCurrentWork(int deskId)
        {
            var desk = await _uow.Desks.GetByIdAsync(deskId);
            desk.IsBusy = false;
            await _uow.SaveAsync();
        }
    }
}
