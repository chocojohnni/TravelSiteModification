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
        public IActionResult SearchFlights(string origin, string destination)
        {
            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
            {
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("FlightOrigin", origin);
            HttpContext.Session.SetString("FlightDestination", destination);

            return RedirectToAction("Index", "Flights");
        }

        [HttpPost]
        public IActionResult SearchCars(string pickupLocation, string dropoffLocation, string pickupDate, string dropoffDate)
        {
            HttpContext.Session.SetString("CarPickupLocation", pickupLocation);
            HttpContext.Session.SetString("CarDropoffLocation", dropoffLocation);
            HttpContext.Session.SetString("CarPickupDate", pickupDate);
            HttpContext.Session.SetString("CarDropoffDate", dropoffDate);

            return RedirectToAction("Index", "Cars");
        }

        [HttpPost]
        public IActionResult SearchEvents(string eventLocation)
        {
            HttpContext.Session.SetString("EventLocation", eventLocation);
            return RedirectToAction("Index", "Events");
        }

        [HttpPost]
        public IActionResult SearchPackages(string origin, string destination, string date)
        {
            HttpContext.Session.SetString("PackageOrigin", origin);
            HttpContext.Session.SetString("PackageDestination", destination);
            HttpContext.Session.SetString("PackageDate", date);

            return RedirectToAction("Index", "Packages");
        }
    }
}
