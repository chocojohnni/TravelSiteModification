using Microsoft.AspNetCore.Mvc;

namespace TravelSiteModification.Controllers
{
    public class CarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
