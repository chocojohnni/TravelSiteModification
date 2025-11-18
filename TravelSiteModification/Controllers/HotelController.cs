using Microsoft.AspNetCore.Mvc;

namespace TravelSiteModification.Controllers
{
    public class HotelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
