using Microsoft.AspNetCore.Mvc;
using QMS.Application.Services;

namespace QMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeskController : ControllerBase
    {

        private readonly IDeskService _deskService;

        public DeskController(IDeskService deskService)
        {
            _deskService = deskService;
        }

        [HttpPost("{id}/call-next")]
        public async Task<IActionResult> CallNext(int id)
        {
            var ticket = await _deskService.CallNext(id);

            if (ticket == null)
                return Ok(new { message = "Növbədə gözləyən müştəri yoxdur." });

            return Ok(new
            {
                ticketNumber = ticket.TicketNumber,
                customer = ticket.CustomerFullName,
                service = ticket.ServiceType
            });
        }
    }
}
