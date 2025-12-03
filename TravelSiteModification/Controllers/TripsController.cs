using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models.Trips;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class TripsController : Controller
    {
        private readonly DBConnect db;

        public TripsController()
        {
            db = new DBConnect();
        }

        public IActionResult Index()
        {
            TripsViewModel model = new TripsViewModel();

            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                model.IsLoggedIn = false;
                return View(model);
            }

            model.IsLoggedIn = true;

            LoadHotelBookings(userId.Value, model);
            LoadFlightBookings(userId.Value, model);
            LoadCarBookings(userId.Value, model);
            LoadEventBookings(userId.Value, model);
            LoadVacationPackages(userId.Value, model);

            return View(model);
        }

        private void LoadHotelBookings(int userId, TripsViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetUserHotelBookings";
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    HotelBookingItem item = new HotelBookingItem();
                    item.HotelName = row["HotelName"].ToString();
                    item.RoomType = row["RoomType"].ToString();
                    item.Status = row["Status"].ToString();
                    item.TotalPrice = row["TotalPrice"].ToString();
                    item.CheckInDate = row["CheckInDate"].ToString();
                    item.CheckOutDate = row["CheckOutDate"].ToString();

                    model.HotelBookings.Add(item);
                }
            }

            model.HasHotelBookings = model.HotelBookings.Count > 0;
        }

        private void LoadFlightBookings(int userId, TripsViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetUserFlightBookings";
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    FlightBookingItem item = new FlightBookingItem();
                    item.AirlineName = row["AirlineName"].ToString();
                    item.DepartureCity = row["DepartureCity"].ToString();
                    item.ArrivalCity = row["ArrivalCity"].ToString();
                    item.DepartureTime = row["DepartureTime"].ToString();
                    item.ArrivalTime = row["ArrivalTime"].ToString();
                    item.Status = row["Status"].ToString();
                    item.TotalPrice = row["TotalPrice"].ToString();

                    model.FlightBookings.Add(item);
                }
            }

            model.HasFlightBookings = model.FlightBookings.Count > 0;
        }

        private void LoadCarBookings(int userId, TripsViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetUserCarBookings";
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    CarBookingItem item = new CarBookingItem();
                    item.CarModel = row["CarModel"].ToString();
                    item.AgencyName = row["AgencyName"].ToString();
                    item.PickupLocationCode = row["PickupLocationCode"].ToString();
                    item.DropoffLocationCode = row["DropoffLocationCode"].ToString();
                    item.PickupDate = row["PickupDate"].ToString();
                    item.DropoffDate = row["DropoffDate"].ToString();
                    item.Status = row["Status"].ToString();
                    item.TotalPrice = row["TotalPrice"].ToString();

                    model.CarBookings.Add(item);
                }
            }

            model.HasCarBookings = model.CarBookings.Count > 0;
        }

        private void LoadEventBookings(int userId, TripsViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetUserEventBookings";
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    EventBookingItem item = new EventBookingItem();
                    item.EventName = row["EventName"].ToString();
                    item.EventLocation = row["EventLocation"].ToString();
                    item.EventDate = row["EventDate"].ToString();
                    item.Status = row["Status"].ToString();
                    item.TotalPrice = row["TotalPrice"].ToString();

                    model.EventBookings.Add(item);
                }
            }

            model.HasEventBookings = model.EventBookings.Count > 0;
        }

        private void LoadVacationPackages(int userId, TripsViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetVacationPackagesByUserID";
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    VacationPackageItem item = new VacationPackageItem();
                    item.PackageName = row["PackageName"].ToString();
                    item.Status = row["Status"].ToString();
                    item.TotalCost = row["TotalCost"].ToString();
                    item.StartDate = row["StartDate"].ToString();
                    item.EndDate = row["EndDate"].ToString();

                    model.VacationPackages.Add(item);
                }
            }

            model.HasPackages = model.VacationPackages.Count > 0;
        }
    }
}
