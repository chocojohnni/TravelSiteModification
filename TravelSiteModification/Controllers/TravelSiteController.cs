using Microsoft.AspNetCore.Mvc;

namespace TravelSiteModification.Controllers
{
    public class TravelSiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
