using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TravelSiteModification.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.FirstName = HttpContext.Session.GetString("UserFirstName");
            return View();
        }

        // HOTEL SEARCH → redirects to HotelsController
        [HttpPost]
        public IActionResult SearchHotels(string destination, string checkIn, string checkOut)
        {
            HttpContext.Session.SetString("HotelDestination", destination ?? "");
            HttpContext.Session.SetString("CheckInDate", checkIn ?? "");
            HttpContext.Session.SetString("CheckOutDate", checkOut ?? "");

            return RedirectToAction("Index", "Hotels");
        }

        // FLIGHT SEARCH → redirects to FlightsController
        [HttpPost]
        public IActionResult SearchFlights(string origin, string destination)
        {
            HttpContext.Session.SetString("FlightOrigin", origin ?? "");
            HttpContext.Session.SetString("FlightDestination", destination ?? "");

            return RedirectToAction("Index", "Flights");
        }

        // CAR SEARCH → redirects to CarsController
        [HttpPost]
        public IActionResult SearchCars(string pickupLocation, string dropoffLocation,
                                        string pickupDate, string dropoffDate)
        {
            HttpContext.Session.SetString("CarPickupLocation", pickupLocation ?? "");
            HttpContext.Session.SetString("CarDropoffLocation", dropoffLocation ?? "");
            HttpContext.Session.SetString("CarPickupDate", pickupDate ?? "");
            HttpContext.Session.SetString("CarDropoffDate", dropoffDate ?? "");

            return RedirectToAction("Index", "Cars");
        }

        // EVENTS SEARCH
        [HttpPost]
        public IActionResult SearchEvents(string eventLocation, string eventDate)
        {
            HttpContext.Session.SetString("EventLocation", eventLocation ?? "");
            HttpContext.Session.SetString("EventDate", eventDate ?? "");

            return RedirectToAction("Index", "Events");
        }

        // PACKAGES SEARCH
        [HttpPost]
        public IActionResult SearchPackages(string origin, string destination, string date)
        {
            HttpContext.Session.SetString("PackageOrigin", origin ?? "");
            HttpContext.Session.SetString("PackageDestination", destination ?? "");
            HttpContext.Session.SetString("PackageDate", date ?? "");

            return RedirectToAction("Index", "Packages");
        }
    }
}
