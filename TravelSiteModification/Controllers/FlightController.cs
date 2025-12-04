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
        private readonly FlightsAPIAccess flightsApi;

        public FlightController(FlightsAPIAccess flightApiAccess)
        {
            db = new DBConnect();
            flightsApi = flightApiAccess;
        }

        public IActionResult Index()
        {
            //testing
            return View();
        }

        [HttpGet]
        public IActionResult Book(int flightId)
        {
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

            FlightBookingViewModel model = new FlightBookingViewModel();
            model.FlightId = flightId;

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                decimal price = 0;
                if (row["Price"] != DBNull.Value)
                {
                    price = Convert.ToDecimal(row["Price"]);
                }
                model.Price = price;

                ViewBag.AirlineName = row["AirlineName"].ToString();
                ViewBag.DepartureCity = row["DepartureCity"].ToString();
                ViewBag.ArrivalCity = row["ArrivalCity"].ToString();
                ViewBag.DepartureTime = Convert.ToDateTime(row["DepartureTime"]);
                ViewBag.ArrivalTime = Convert.ToDateTime(row["ArrivalTime"]);
            }
            else
            {
                model.Price = 0;

                ViewBag.AirlineName = "Selected Flight";
                ViewBag.DepartureCity = "";
                ViewBag.ArrivalCity = "";
                ViewBag.DepartureTime = DateTime.Now;
                ViewBag.ArrivalTime = DateTime.Now;
            }

            string firstName = HttpContext.Session.GetString("UserFirstName");
            if (!string.IsNullOrEmpty(firstName))
            {
                model.FirstName = firstName;
            }

            string email = HttpContext.Session.GetString("UserEmail");
            if (!string.IsNullOrEmpty(email))
            {
                model.Email = email;
            }

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

            if (!ModelState.IsValid)
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
                    try
                    {
                        int packageId = GetOrCreateOpenVacationPackage(userIdValue, model.Price);

                        SqlCommand pkgCmd = new SqlCommand();
                        pkgCmd.CommandType = CommandType.StoredProcedure;
                        pkgCmd.CommandText = "spInsertPackageFlight";
                        pkgCmd.Parameters.AddWithValue("@PackageID", packageId);
                        pkgCmd.Parameters.AddWithValue("@FlightID", model.FlightId);

                        db.DoUpdateUsingCmdObj(pkgCmd);

                        ViewBag.IsSuccess = true;
                        ViewBag.StatusMessage =
                            "Your flight has been booked and added to your vacation package.";
                    }
                    catch (Exception packageEx)
                    {
                        ViewBag.IsSuccess = true;
                        ViewBag.StatusMessage =
                            "Your flight has been booked, but it could not be added to your vacation package: " +
                            packageEx.Message;
                    }
                }
                else
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.StatusMessage =
                        "There was a problem saving your booking. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.StatusMessage =
                    "Database error while saving your booking: " + ex.Message;
            }

            return View("~/Views/TravelSite/FlightBooking.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Find()
        {
            FlightSearchViewModel model = new FlightSearchViewModel();

            // Optional defaults:
            // model.DepCity = "New York";
            // model.ArrCity = "Los Angeles";

            model.Carriers = await flightsApi.GetAllCarriersAsync();

            return View("Find", model);
        }

        [HttpPost]
        public async Task<IActionResult> Find(FlightSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Carriers = await flightsApi.GetAllCarriersAsync();
                return View("Find", model);
            }

            FlightRequirements req = new FlightRequirements
            {
                AirlineID = model.AirlineID,
                MaxPrice = model.MaxPrice,
                NonStop = model.NonStop,
                FirstClass = model.FirstClass
            };

            try
            {
                List<Flight> flights = await flightsApi.FindFlightsAsync(
                    model.DepCity,
                    model.DepState,
                    model.ArrCity,
                    model.ArrState,
                    req);

                model.Flights = flights;
                model.Carriers = await flightsApi.GetAllCarriersAsync();

                if (flights.Count == 0)
                {
                    model.ErrorMessage = "No flights found for the given criteria.";
                }

                return View("Find", model);
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Error calling Flights API: " + ex.Message;
                model.Carriers = await flightsApi.GetAllCarriersAsync();
                return View("Find", model);
            }
        }

        private int GetOrCreateOpenVacationPackage(int userId, decimal additionalCost)
        {
            int packageId = 0;

            int? sessionPackageId = HttpContext.Session.GetInt32("CurrentPackageID");
            if (sessionPackageId.HasValue)
            {
                packageId = sessionPackageId.Value;
            }

            DBConnect dbConnect = new DBConnect();

            if (packageId == 0)
            {
                SqlCommand findCmd = new SqlCommand();
                findCmd.CommandType = CommandType.Text;
                findCmd.CommandText =
                    "SELECT TOP 1 PackageID " +
                    "FROM VacationPackage " +
                    "WHERE UserID = @UserID AND Status = @Status " +
                    "ORDER BY DateCreated DESC";

                findCmd.Parameters.AddWithValue("@UserID", userId);
                findCmd.Parameters.AddWithValue("@Status", "Building");

                DataSet ds = dbConnect.GetDataSetUsingCmdObj(findCmd);

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    packageId = Convert.ToInt32(ds.Tables[0].Rows[0]["PackageID"]);
                }
            }

            if (packageId == 0)
            {
                SqlCommand insertCmd = new SqlCommand();
                insertCmd.CommandType = CommandType.StoredProcedure;
                insertCmd.CommandText = "InsertVacationPackage";

                insertCmd.Parameters.AddWithValue("@UserID", userId);
                insertCmd.Parameters.AddWithValue("@PackageName", "My Vacation Package");
                insertCmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                insertCmd.Parameters.AddWithValue("@EndDate", DateTime.Today.AddDays(7));
                insertCmd.Parameters.AddWithValue("@TotalCost", additionalCost);
                insertCmd.Parameters.AddWithValue("@Status", "Building");

                SqlParameter outputParam =
                    new SqlParameter("@NewPackageID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                insertCmd.Parameters.Add(outputParam);

                dbConnect.DoUpdateUsingCmdObj(insertCmd);

                packageId = Convert.ToInt32(outputParam.Value);
            }
            else
            {
                SqlCommand updateCmd = new SqlCommand();
                updateCmd.CommandType = CommandType.Text;
                updateCmd.CommandText =
                    "UPDATE VacationPackage " +
                    "SET TotalCost = TotalCost + @Amount " +
                    "WHERE PackageID = @PackageID";

                updateCmd.Parameters.AddWithValue("@Amount", additionalCost);
                updateCmd.Parameters.AddWithValue("@PackageID", packageId);

                dbConnect.DoUpdateUsingCmdObj(updateCmd);
            }

            HttpContext.Session.SetInt32("CurrentPackageID", packageId);
            return packageId;
        }
    }
}
