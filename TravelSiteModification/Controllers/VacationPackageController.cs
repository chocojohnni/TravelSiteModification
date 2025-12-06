using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Utilities;
using TravelSiteModification.Models;


namespace TravelSiteModification.Controllers
{
    public class VacationPackageController : Controller
    {
        private readonly DBConnect db;

        public VacationPackageController()
        {
            db = new DBConnect();
        }

        [HttpGet]
        public IActionResult Current()
        {
            int? userIdNullable = HttpContext.Session.GetInt32("UserID");
            if (!userIdNullable.HasValue)
            {
                string redirectUrl = Url.Action("Current", "VacationPackage");
                HttpContext.Session.SetString("RedirectAfterLogin", redirectUrl);
                return RedirectToAction("Login", "Account");
            }

            int userId = userIdNullable.Value;

            CurrentPackageViewModel model = new CurrentPackageViewModel();

            // Load the package
            SqlCommand pkgCmd = new SqlCommand();
            pkgCmd.CommandType = CommandType.StoredProcedure;
            pkgCmd.CommandText = "TP_GetVacationPackage";
            pkgCmd.Parameters.AddWithValue("@UserID", userId);

            DataSet pkgDs = db.GetDataSetUsingCmdObj(pkgCmd);

            if (pkgDs == null || pkgDs.Tables.Count == 0 || pkgDs.Tables[0].Rows.Count == 0)
            {
                model.HasPackage = false;
                return View("~/Views/VacationPackage/Current.cshtml", model);
            }

            DataRow pkgRow = pkgDs.Tables[0].Rows[0];

            model.HasPackage = true;
            model.PackageId = Convert.ToInt32(pkgRow["PackageID"]);
            model.PackageName = pkgRow["PackageName"].ToString();
            model.Status = pkgRow["Status"].ToString();

            if (pkgRow["TotalCost"] != DBNull.Value)
            {
                model.TotalCost = Convert.ToDecimal(pkgRow["TotalCost"]);
            }

            model.StartDate = Convert.ToDateTime(pkgRow["StartDate"]);
            model.EndDate = Convert.ToDateTime(pkgRow["EndDate"]);

            // Load all flights
            SqlCommand flightsCmd = new SqlCommand();
            flightsCmd.CommandType = CommandType.StoredProcedure;
            flightsCmd.CommandText = "TP_GetPackageFlightDetails";
            flightsCmd.Parameters.AddWithValue("@PackageID", model.PackageId);

            DataSet flightsDs = db.GetDataSetUsingCmdObj(flightsCmd);

            if (flightsDs != null && flightsDs.Tables.Count > 0)
            {
                foreach (DataRow row in flightsDs.Tables[0].Rows)
                {
                    PackageFlightItem item = new PackageFlightItem();

                    item.PackageFlightId = Convert.ToInt32(row["PackageFlightID"]);
                    item.FlightId = Convert.ToInt32(row["FlightID"]);
                    item.AirlineName = row["AirlineName"].ToString();
                    item.DepartureCity = row["DepartureCity"].ToString();
                    item.ArrivalCity = row["ArrivalCity"].ToString();
                    item.DepartureTime = Convert.ToDateTime(row["DepartureTime"]);
                    item.ArrivalTime = Convert.ToDateTime(row["ArrivalTime"]);

                    if (row["Price"] != DBNull.Value)
                    {
                        item.Price = Convert.ToDecimal(row["Price"]);
                    }

                    model.Flights.Add(item);
                }
            }

            SqlCommand hotelsCmd = new SqlCommand();
            hotelsCmd.CommandType = CommandType.StoredProcedure;
            hotelsCmd.CommandText = "TP_GetPackageHotelDetails";
            hotelsCmd.Parameters.AddWithValue("@PackageID", model.PackageId);

            DataSet hotelsDs = db.GetDataSetUsingCmdObj(hotelsCmd);

            if (hotelsDs != null && hotelsDs.Tables.Count > 0)
            {
                foreach (DataRow row in hotelsDs.Tables[0].Rows)
                {
                    PackageHotelItem hotel = new PackageHotelItem();

                    hotel.PackageHotelId = Convert.ToInt32(row["PackageHotelID"]);
                    hotel.HotelId = Convert.ToInt32(row["HotelID"]);
                    hotel.RoomId = Convert.ToInt32(row["RoomID"]);

                    hotel.HotelName = row["HotelName"].ToString();
                    hotel.City = row["City"].ToString();
                    hotel.State = row["State"].ToString();

                    if (row["CheckInDate"] != DBNull.Value)
                    {
                        hotel.CheckInDate = Convert.ToDateTime(row["CheckInDate"]);
                    }

                    if (row["CheckOutDate"] != DBNull.Value)
                    {
                        hotel.CheckOutDate = Convert.ToDateTime(row["CheckOutDate"]);
                    }

                    if (row["TotalPrice"] != DBNull.Value)
                    {
                        hotel.TotalPrice = Convert.ToDecimal(row["TotalPrice"]);
                    }

                    model.Hotels.Add(hotel);
                }
            }

            SqlCommand carsCmd = new SqlCommand();
            carsCmd.CommandType = CommandType.StoredProcedure;
            carsCmd.CommandText = "TP_GetPackageCarDetails";
            carsCmd.Parameters.AddWithValue("@PackageID", model.PackageId);

            DataSet carsDs = db.GetDataSetUsingCmdObj(carsCmd);

            if (carsDs != null && carsDs.Tables.Count > 0)
            {
                foreach (DataRow row in carsDs.Tables[0].Rows)
                {
                    PackageCarItem car = new PackageCarItem();

                    car.PackageCarId = Convert.ToInt32(row["PackageCarID"]);
                    car.CarId = Convert.ToInt32(row["CarID"]);
                    car.CarModel = row["CarModel"].ToString();
                    car.CarType = row["CarType"].ToString();
                    car.PickupLocationCode = row["PickupLocationCode"].ToString();
                    car.DropoffLocationCode = row["DropoffLocationCode"].ToString();
                    car.AgencyName = row["AgencyName"].ToString();

                    if (row["PricePerDay"] != DBNull.Value)
                    {
                        car.PricePerDay = Convert.ToDecimal(row["PricePerDay"]);
                    }

                    model.Cars.Add(car);
                }
            }

            SqlCommand eventsCmd = new SqlCommand();
            eventsCmd.CommandType = CommandType.StoredProcedure;
            eventsCmd.CommandText = "TP_GetPackageEventDetails";
            eventsCmd.Parameters.AddWithValue("@PackageID", model.PackageId);

            DataSet eventsDs = db.GetDataSetUsingCmdObj(eventsCmd);

            if (eventsDs != null && eventsDs.Tables.Count > 0 && eventsDs.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in eventsDs.Tables[0].Rows)
                {
                    PackageEventItem evt = new PackageEventItem();

                    evt.PackageEventId = Convert.ToInt32(row["PackageEventID"]);
                    evt.EventId = Convert.ToInt32(row["EventID"]);
                    evt.EventName = row["EventName"].ToString();
                    evt.EventLocation = row["EventLocation"].ToString();
                    evt.EventDate = Convert.ToDateTime(row["EventDate"]);

                    if (row["Price"] != DBNull.Value)
                    {
                        evt.Price = Convert.ToDecimal(row["Price"]);
                    }

                    model.Events.Add(evt);
                }
            }

            return View("~/Views/VacationPackage/Current.cshtml", model);
        }

        // POST: /VacationPackage/Confirm
        [HttpPost]
        public IActionResult Confirm(int packageId)
        {
            int? userIdNullable = HttpContext.Session.GetInt32("UserID");
            if (!userIdNullable.HasValue)
            {
                string redirectUrl = Url.Action("Current", "VacationPackage");
                HttpContext.Session.SetString("RedirectAfterLogin", redirectUrl);
                return RedirectToAction("Login", "Account");
            }

            decimal totalCost = 0m;

            SqlCommand flightsCmd = new SqlCommand();
            flightsCmd.CommandType = CommandType.StoredProcedure;
            flightsCmd.CommandText = "TP_GetPackageFlightDetails";
            flightsCmd.Parameters.AddWithValue("@PackageID", packageId);

            DataSet flightsDs = db.GetDataSetUsingCmdObj(flightsCmd);

            if (flightsDs != null && flightsDs.Tables.Count > 0)
            {
                foreach (DataRow row in flightsDs.Tables[0].Rows)
                {
                    if (row["Price"] != DBNull.Value)
                    {
                        decimal price = Convert.ToDecimal(row["Price"]);
                        totalCost = totalCost + price;
                    }
                }
            }

            SqlCommand hotelsCmd = new SqlCommand();
            hotelsCmd.CommandType = CommandType.StoredProcedure;
            hotelsCmd.CommandText = "TP_GetPackageHotelDetails";
            hotelsCmd.Parameters.AddWithValue("@PackageID", packageId);

            DataSet hotelsDs = db.GetDataSetUsingCmdObj(hotelsCmd);

            if (hotelsDs != null && hotelsDs.Tables.Count > 0)
            {
                foreach (DataRow row in hotelsDs.Tables[0].Rows)
                {
                    if (row["TotalPrice"] != DBNull.Value)
                    {
                        decimal price = Convert.ToDecimal(row["TotalPrice"]);
                        totalCost = totalCost + price;
                    }
                }
            }

            SqlCommand carsCmd = new SqlCommand();
            carsCmd.CommandType = CommandType.StoredProcedure;
            carsCmd.CommandText = "TP_GetPackageCarDetails";
            carsCmd.Parameters.AddWithValue("@PackageID", packageId);

            DataSet carsDs = db.GetDataSetUsingCmdObj(carsCmd);

            if (carsDs != null && carsDs.Tables.Count > 0)
            {
                foreach (DataRow row in carsDs.Tables[0].Rows)
                {
                    if (row["PricePerDay"] != DBNull.Value)
                    {
                        decimal price = Convert.ToDecimal(row["PricePerDay"]);
                        totalCost = totalCost + price;
                    }
                }
            }

            SqlCommand eventsCmd = new SqlCommand();
            eventsCmd.CommandType = CommandType.StoredProcedure;
            eventsCmd.CommandText = "TP_GetPackageEventDetails";
            eventsCmd.Parameters.AddWithValue("@PackageID", packageId);

            DataSet eventsDs = db.GetDataSetUsingCmdObj(eventsCmd);
            if (eventsDs != null && eventsDs.Tables.Count > 0)
            {
                foreach (DataRow row in eventsDs.Tables[0].Rows)
                {
                    if (row["Price"] != DBNull.Value)
                    {
                        totalCost = totalCost + Convert.ToDecimal(row["Price"]);
                    }
                }
            }

            SqlCommand confirmCmd = new SqlCommand();
            confirmCmd.CommandType = CommandType.StoredProcedure;
            confirmCmd.CommandText = "TP_ConfirmVacationPackage";
            confirmCmd.Parameters.AddWithValue("@PackageID", packageId);
            confirmCmd.Parameters.AddWithValue("@TotalCost", totalCost);

            db.DoUpdateUsingCmdObj(confirmCmd);

            HttpContext.Session.Remove("CurrentPackageID");

            TempData["PackageConfirmationMessage"] = "Your vacation package has been confirmed!";

            return RedirectToAction("Index", "Trips");
        }
    }
}
