using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TravelSiteModification.Models;
using TravelSiteModification.Services;

namespace TravelSiteModification.Controllers
{
    public class CarBookingController : Controller
    {
        private readonly CarAPIService api;

        public CarBookingController(CarAPIService apiService)
        {
            api = apiService;
        }

        // GET /CarBooking
        public async Task<IActionResult> Index()
        {
            // Require login
            if (HttpContext.Session.GetString("UserFirstName") == null)
            {
                HttpContext.Session.SetString("RedirectAfterLogin", "CarBooking");
                return RedirectToAction("Login", "Account");
            }

            // Require car selection
            int? carId = HttpContext.Session.GetInt32("SelectedCarID");
            string pickup = HttpContext.Session.GetString("CarPickupDate");
            string dropoff = HttpContext.Session.GetString("CarDropoffDate");

            if (carId == null || pickup == null || dropoff == null)
            {
                return RedirectToAction("Index", "Cars");
            }

            DateTime pickupDate = Convert.ToDateTime(pickup);
            DateTime dropoffDate = Convert.ToDateTime(dropoff);

            int totalDays = (int)(dropoffDate - pickupDate).TotalDays;
            if (totalDays <= 0) totalDays = 1;

            CarBookingViewModel model =
                await BuildCarBookingModel(carId.Value, pickupDate, dropoffDate, totalDays);

            return View(model);
        }

        // POST /CarBooking
        [HttpPost]
        public async Task<IActionResult> Index(CarBookingViewModel model)
        {
            int? carId = HttpContext.Session.GetInt32("SelectedCarID");
            string pickup = HttpContext.Session.GetString("CarPickupDate");
            string dropoff = HttpContext.Session.GetString("CarDropoffDate");

            if (carId == null || pickup == null || dropoff == null)
            {
                model.StatusMessage = "<p class='alert alert-danger'>Session data missing.</p>";
                return View(model);
            }

            DateTime pickupDate = Convert.ToDateTime(pickup);
            DateTime dropoffDate = Convert.ToDateTime(dropoff);
            int days = (int)(dropoffDate - pickupDate).TotalDays;
            if (days <= 0) days = 1;

            CarBookingViewModel rebuilt =
                await BuildCarBookingModel(carId.Value, pickupDate, dropoffDate, days);

            // Copy posted fields
            rebuilt.FirstName = model.FirstName;
            rebuilt.LastName = model.LastName;
            rebuilt.Email = model.Email;
            rebuilt.CardNumber = model.CardNumber;
            rebuilt.ExpiryDate = model.ExpiryDate;
            rebuilt.CVV = model.CVV;

            // Validation
            if (string.IsNullOrWhiteSpace(rebuilt.FirstName) ||
                string.IsNullOrWhiteSpace(rebuilt.Email))
            {
                rebuilt.StatusMessage =
                    "<p class='text-danger'>Please enter First Name and Email.</p>";
                return View(rebuilt);
            }

            decimal finalPrice =
                HttpContext.Session.GetString("CarFinalPrice") != null
                ? Convert.ToDecimal(HttpContext.Session.GetString("CarFinalPrice"))
                : rebuilt.TotalCost;

            string result = await SubmitReservationToApi(carId.Value, pickupDate, dropoffDate, finalPrice, rebuilt);

            if (result == "Success")
            {
                rebuilt.BookingConfirmed = true;
                rebuilt.StatusMessage =
                    "<p class='alert alert-success'><strong>Booking Confirmed!</strong></p>";

                // Remove cart data
                HttpContext.Session.Remove("SelectedCarID");
                HttpContext.Session.Remove("CarFinalPrice");
            }
            else
            {
                rebuilt.BookingConfirmed = false;
                rebuilt.StatusMessage = result;
            }

            return View(rebuilt);
        }

        // -------------------------------------------------------------------
        // BUILD BOOKING MODEL (API version)
        // -------------------------------------------------------------------
        private async Task<CarBookingViewModel> BuildCarBookingModel(
            int carId, DateTime pickup, DateTime dropoff, int days)
        {
            CarBookingViewModel model = new CarBookingViewModel();

            // Step 1: Get all cars in city
            string sessionCityCode = HttpContext.Session.GetString("CarPickupLocation");
            string cityCode = sessionCityCode;
            List<CarAPIModel> cars = await api.FindCarsAsync(cityCode, "", 0, 2000);

            CarAPIModel selected = cars.Find(c => c.CarID == carId);

            if (selected == null)
            {
                model.StatusMessage = "<p class='alert alert-danger'>Car not found.</p>";
                return model;
            }

            decimal pricePerDay = selected.DailyRate;
            decimal total = pricePerDay * days;

            HttpContext.Session.SetString("CarFinalPrice", total.ToString());

            model.SelectedCarID = carId;
            model.CarModel = selected.CarModel;
            model.CarType = selected.CarType;
            model.AgencyName = "";
            model.PickupLocation = selected.PickupLocationCode;
            model.DropoffLocation = selected.DropoffLocationCode;
            model.PickupDateDisplay = pickup.ToShortDateString();
            model.DropoffDateDisplay = dropoff.ToShortDateString();
            model.TotalDays = days;
            model.TotalCost = total;

            return model;
        }


        // -------------------------------------------------------------------
        // API Reservation Submission
        // -------------------------------------------------------------------
        private async Task<string> SubmitReservationToApi(
            int carId, DateTime pickup, DateTime dropoff, decimal finalPrice, CarBookingViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null || userId <= 0)
            {
                return "<p class='alert alert-danger'>❌ User ID missing.</p>";
            }

            ReservationRequest req = new ReservationRequest();
            req.UserID = userId.Value;
            req.CarID = carId;
            req.PickupDate = pickup;
            req.DropoffDate = dropoff;
            req.TotalAmount = finalPrice;
            req.FirstName = model.FirstName;
            req.LastName = model.LastName ?? "";
            req.Email = model.Email;
            req.TravelSiteID = 1;
            req.TravelSiteAPIToken = "TEST-TOKEN-123";

            var response = await api.ReserveCarAsync(req);

            if (response.Success)
                return "Success";

            return "<p class='alert alert-danger'>" + response.Message + "</p>";
        }

        // -------------------------------------------------------------------
        // Helper: Convert airport→city
        // -------------------------------------------------------------------
        private string ConvertToApiCity(string code)
        {
            if (code == "NYC") return "New York";
            if (code == "LAX") return "Los Angeles";
            if (code == "MIA") return "Miami";
            if (code == "SEA") return "Seattle";
            return code;
        }
    }
}
