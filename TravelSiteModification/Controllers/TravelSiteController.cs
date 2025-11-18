using Microsoft.AspNetCore.Mvc;

namespace TravelSiteModification.Controllers
{
    public class TravelSiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchHotels(string destination, string checkInDate, string checkOutDate)
        {
            if (string.IsNullOrEmpty(destination) || string.IsNullOrEmpty(checkInDate) || string.IsNullOrEmpty(checkOutDate))
            {
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("HotelDestination", destination);
            HttpContext.Session.SetString("CheckInDate", checkInDate);
            HttpContext.Session.SetString("CheckOutDate", checkOutDate);

            return RedirectToAction("Index", "Hotels");
        }

        [HttpPost]
        public IActionResult SearchFlights(string flightDestination)
        {

        }
    }
}
