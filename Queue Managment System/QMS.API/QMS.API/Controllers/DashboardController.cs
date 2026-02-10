using Microsoft.AspNetCore.Mvc;
using QMS.Application.Services;

namespace QMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("live-status")]
        public async Task<IActionResult> GetLiveStatus()
        {
            var result = await _dashboardService.GetBankLiveStatusAsync();

            if (result == null)
                return NotFound("Məlumat tapılmadı.");

            return Ok(result);
        }
    }
}
