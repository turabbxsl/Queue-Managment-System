using Microsoft.AspNetCore.Mvc;
using QMS.Application.Dtos;
using QMS.Application.Services;

namespace QMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }



        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTicket([FromBody] CreateTicketDto request)
        {
            if (string.IsNullOrEmpty(request.CustomerFullName))
                return BadRequest("Müştəri adı boş ola bilməz.");

            var ticket = await _ticketService.CreateTicketAsync(request);

            return Ok(new
            {
                TicketNumber = ticket.TicketNumber,
                QueuePosition = ticket.Desk.QueueCount,
                AssignedDesk = ticket.Desk.DeskName,
                CreatedAt = ticket.CreatedAt.ToString("HH:mm")
            });
        }



        [HttpPost("transfer")]
        public async Task<IActionResult> TransferTicket([FromQuery] int ticketId, [FromQuery] int targetDeskId)
        {
            var result = await _ticketService.TransferTicketAsync(ticketId, targetDeskId);

            if (!result)
                return BadRequest("Transfer mümkün olmadı. Bilet artıq həmin masadadır və ya tapılmadı.");

            return Ok(new { message = "Müştəri uğurla digər masanın növbəsinə keçirildi." });
        }



        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelTicket(int id)
        {
            var result = await _ticketService.CancelTicketAsync(id);

            if (!result)
                return BadRequest("Bilet tapılmadı və ya artıq xidmət prosesindədir.");

            return Ok(new { message = "Növbə uğurla ləğv edildi." });
        }

    }
}
