using Microsoft.AspNetCore.Mvc;

namespace QMS.UI.Controllers
{
    public class MonitorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
