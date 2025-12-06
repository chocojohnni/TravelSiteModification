using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Hubs;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class CarBookingController : Controller
    {
        private readonly DBConnect db = new DBConnect();
        private readonly IHubContext<NotificationHub> _hub;

        public CarBookingController(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        // GET /CarBooking
        public IActionResult Index()
        {
            // Require login
            if (HttpContext.Session.GetString("UserFirstName") == null)
            {
                HttpContext.Session.SetString("RedirectAfterLogin", "CarBooking");
                return RedirectToAction("Login", "Account");
            }

            // Require selected car + dates
            if (!SessionIsValid())
                return RedirectToAction("Index", "Cars");

            // Build model just like WebForms Page_Load → BindCarDetails
            CarBookingViewModel model = LoadBookingDetails();
            return View(model);
        }

        // POST /CarBooking
        [HttpPost]
        public IActionResult Index(CarBookingViewModel model)
        {
            if (!SessionIsValid())
            {
                model.Status = "<p class='alert alert-danger'>Session data missing.</p>";
                return View(model);
            }

            // Reload DB fields so users can't tamper with prices
            CarBookingViewModel rebuilt = LoadBookingDetails();

            // Copy form fields into authoritative rebuilt model
            rebuilt.FirstName = model.FirstName;
            rebuilt.LastName = model.LastName;
            rebuilt.Email = model.Email;
            rebuilt.CardNumber = model.CardNumber;
            rebuilt.ExpiryDate = model.ExpiryDate;
            rebuilt.CVV = model.CVV;

            // Validate fields
            if (string.IsNullOrWhiteSpace(rebuilt.FirstName) ||
                string.IsNullOrWhiteSpace(rebuilt.Email))
            {
                rebuilt.Status = "<p class='text-danger'>Please enter First Name and Email.</p>";
                return View(rebuilt);
            }

            // Save in DB
            string result = InsertCarBooking(rebuilt);

            if (result == "Success")
            {
                rebuilt.BookingConfirmed = true;
                rebuilt.Status =
                    "<p class='alert alert-success'>🎉 <strong>Booking Confirmed!</strong></p>";

                // Clear cart session
                HttpContext.Session.Remove("SelectedCarID");
                HttpContext.Session.Remove("CarFinalPrice");
            }
            else
            {
                rebuilt.BookingConfirmed = false;
                rebuilt.Status = result;
            }

            return View(rebuilt);
        }

        // ------------------------- HELPERS -------------------------------

        private bool SessionIsValid()
        {
            return HttpContext.Session.GetInt32("SelectedCarID") != null &&
                   HttpContext.Session.GetString("CarPickupDate") != null &&
                   HttpContext.Session.GetString("CarDropoffDate") != null;
        }

        private CarBookingViewModel LoadBookingDetails()
        {
            int carId = HttpContext.Session.GetInt32("SelectedCarID") ?? 0;
            DateTime pickup = Convert.ToDateTime(HttpContext.Session.GetString("CarPickupDate"));
            DateTime dropoff = Convert.ToDateTime(HttpContext.Session.GetString("CarDropoffDate"));

            int totalDays = (int)(dropoff - pickup).TotalDays;
            if (totalDays <= 0) totalDays = 1;

            CarBookingViewModel model = new CarBookingViewModel();

            // DB Lookup
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCarAndAgencyDetails";
            cmd.Parameters.AddWithValue("@CarID", carId);

            DataTable dt = db.GetDataSetUsingCmdObj(cmd).Tables[0];

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                model.CarModel = row["CarModel"].ToString();
                model.CarType = row["CarType"].ToString();
                model.AgencyName = row["AgencyName"].ToString();
                model.PickupLocation = row["PickupLocationCode"].ToString();
                model.DropoffLocation = row["DropoffLocationCode"].ToString();

                decimal pricePerDay = Convert.ToDecimal(row["PricePerDay"]);
                decimal cost = pricePerDay * totalDays;

                HttpContext.Session.SetString("CarFinalPrice", cost.ToString());

                model.TotalCost = cost;
                model.TotalDays = totalDays;
                model.PickupDate = pickup.ToShortDateString();
                model.DropoffDate = dropoff.ToShortDateString();
            }

            return model;
        }

        private string InsertCarBooking(CarBookingViewModel model)
        {
            int userID = HttpContext.Session.GetInt32("UserID") ?? 0;
            if (userID <= 0)
                return "<p class='alert alert-danger'>❌ UserID missing.</p>";

            int carId = HttpContext.Session.GetInt32("SelectedCarID") ?? 0;
            DateTime pickup = Convert.ToDateTime(HttpContext.Session.GetString("CarPickupDate"));
            DateTime dropoff = Convert.ToDateTime(HttpContext.Session.GetString("CarDropoffDate"));
            decimal finalPrice = Convert.ToDecimal(HttpContext.Session.GetString("CarFinalPrice"));

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddCarBooking";

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@CarID", carId);
            cmd.Parameters.AddWithValue("@PickupDate", pickup);
            cmd.Parameters.AddWithValue("@DropoffDate", dropoff);
            cmd.Parameters.AddWithValue("@TotalAmount", finalPrice);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
            cmd.Parameters.AddWithValue("@Email", model.Email);

            try
            {
                int rows = db.DoUpdateUsingCmdObj(cmd);

                if (rows > 0)
                {
                    // Update availability
                    SqlCommand updateCar = new SqlCommand();
                    updateCar.CommandType = CommandType.StoredProcedure;
                    updateCar.CommandText = "UpdateCarAvailability";
                    updateCar.Parameters.AddWithValue("@CarID", carId);
                    db.DoUpdateUsingCmdObj(updateCar);

                    _hub.Clients.All.SendAsync(
                        "ReceiveNotification",
                        $"{model.FirstName} just booked a {model.CarModel} ({model.CarType}) for {pickup:MM/dd} - {dropoff:MM/dd}!"
                    );

                    return "Success";
                }
                else
                {
                    return "<p class='alert alert-danger'>❌ No rows added.</p>";
                }
            }
            catch (Exception ex)
            {
                return $"<p class='alert alert-danger'>❌ Error: {ex.Message}</p>";
            }
        }


        // Called when user clicks “Book This Car”
        [HttpPost]
        public IActionResult StartBooking(int carId)
        {
            HttpContext.Session.SetInt32("SelectedCarID", carId);
            return RedirectToAction("Index");
        }
    }
}
