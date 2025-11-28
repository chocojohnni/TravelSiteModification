using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using TravelSiteModification.Services;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class FlightController : Controller
    {
        private readonly DBConnect db;
        private readonly FlightsAPIClient flightsApiClient;
        public FlightController(FlightsAPIClient flightsClient)
        {
            db = new DBConnect();
            flightsApiClient = flightsClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Book(int flightId)
        {
            // Make sure the user is logged in
            int? userIdNullable = HttpContext.Session.GetInt32("UserID");
            int userIdValue = 0;

            if (userIdNullable.HasValue)
            {
                userIdValue = userIdNullable.Value;
            }
            else
            {
                string redirectUrl = Url.Action("Book", "Flight", new { flightId = flightId });
                HttpContext.Session.SetString("RedirectAfterLogin", redirectUrl);

                return RedirectToAction("Login", "Account");
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetFlightByID";
            cmd.Parameters.AddWithValue("@FlightID", flightId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Index", "TravelSite");
            }

            DataRow row = ds.Tables[0].Rows[0];

            decimal price = 0;

            if (row["Price"] != DBNull.Value)
            {
                price = Convert.ToDecimal(row["Price"]);
            }

            FlightBookingViewModel model = new FlightBookingViewModel();
            model.FlightId = flightId;
            model.Price = price;

            string firstName = HttpContext.Session.GetString("UserFirstName");
            if (String.IsNullOrEmpty(firstName) == false)
            {
                model.FirstName = firstName;
            }

            string email = HttpContext.Session.GetString("UserEmail");
            if (String.IsNullOrEmpty(email) == false)
            {
                model.Email = email;
            }

            ViewBag.AirlineName = row["AirlineName"].ToString();
            ViewBag.DepartureCity = row["DepartureCity"].ToString();
            ViewBag.ArrivalCity = row["ArrivalCity"].ToString();
            ViewBag.DepartureTime = Convert.ToDateTime(row["DepartureTime"]);
            ViewBag.ArrivalTime = Convert.ToDateTime(row["ArrivalTime"]);

            return View("~/Views/TravelSite/FlightBooking.cshtml", model);
        }

        [HttpPost]
        public IActionResult Book(FlightBookingViewModel model)
        {
            int? userIdNullable = HttpContext.Session.GetInt32("UserID");
            int userIdValue = 0;

            if (userIdNullable.HasValue)
            {
                userIdValue = userIdNullable.Value;
            }
            else
            {
                string redirectUrl = Url.Action("Book", "Flight", new { flightId = model.FlightId });
                HttpContext.Session.SetString("RedirectAfterLogin", redirectUrl);

                return RedirectToAction("Login", "Account");
            }

            SqlCommand reloadCmd = new SqlCommand();
            reloadCmd.CommandType = CommandType.StoredProcedure;
            reloadCmd.CommandText = "GetFlightByID";
            reloadCmd.Parameters.AddWithValue("@FlightID", model.FlightId);

            DataSet reloadDataSet = db.GetDataSetUsingCmdObj(reloadCmd);

            if (reloadDataSet != null &&
                reloadDataSet.Tables.Count > 0 &&
                reloadDataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = reloadDataSet.Tables[0].Rows[0];

                ViewBag.AirlineName = row["AirlineName"].ToString();
                ViewBag.DepartureCity = row["DepartureCity"].ToString();
                ViewBag.ArrivalCity = row["ArrivalCity"].ToString();
                ViewBag.DepartureTime = Convert.ToDateTime(row["DepartureTime"]);
                ViewBag.ArrivalTime = Convert.ToDateTime(row["ArrivalTime"]);

                if (row["Price"] != DBNull.Value && model.Price == 0)
                {
                    decimal priceFromDatabase = Convert.ToDecimal(row["Price"]);
                    model.Price = priceFromDatabase;
                }
            }

            if (ModelState.IsValid == false)
            {
                return View("~/Views/TravelSite/FlightBooking.cshtml", model);
            }

            SqlCommand insertCmd = new SqlCommand();
            insertCmd.CommandType = CommandType.StoredProcedure;
            insertCmd.CommandText = "AddFlightBooking";

            insertCmd.Parameters.AddWithValue("@FlightID", model.FlightId);
            insertCmd.Parameters.AddWithValue("@UserID", userIdValue);
            insertCmd.Parameters.AddWithValue("@FirstName", model.FirstName.Trim());
            insertCmd.Parameters.AddWithValue("@LastName", model.LastName.Trim());
            insertCmd.Parameters.AddWithValue("@Email", model.Email.Trim());
            insertCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);
            insertCmd.Parameters.AddWithValue("@TotalPrice", model.Price);
            insertCmd.Parameters.AddWithValue("@Status", "Pending");

            try
            {
                int rowsAffected = db.DoUpdateUsingCmdObj(insertCmd);

                if (rowsAffected > 0)
                {
                    ViewBag.IsSuccess = true;
                    ViewBag.StatusMessage = "Your flight has been booked successfully.";
                }
                else
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.StatusMessage = "There was a problem saving your booking. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.StatusMessage = "Database error while saving your booking: " + ex.Message;
            }

            return View("~/Views/TravelSite/FlightBooking.cshtml", model);
        }

        [HttpGet]
        public IActionResult Find()
        {
            FlightSearchRequest model = new FlightSearchRequest();
            model.NonStop = true;
            model.FirstClass = false;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Find(FlightSearchRequest model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            try
            {
                List<FlightDto> flights = await flightsApiClient.FindFlightsAsync(model);

                FlightSearchResultsViewModel viewModel = new FlightSearchResultsViewModel();
                viewModel.Search = model;
                viewModel.Flights = flights;

                return View("FindResults", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Error calling Flights API: " + ex.Message);
                return View(model);
            }
        }
    }
}
