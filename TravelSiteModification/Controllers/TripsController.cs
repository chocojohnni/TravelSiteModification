using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models.Trips;
using Utilities;
using ScottPlot;
using System.IO;

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

            SpendingData spendingData = new SpendingData();
            SpendingSummary spendingSummary = spendingData.GetUserSpendingSummary(userId.Value);
            ViewBag.SpendingSummary = spendingSummary;

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

        public IActionResult SpendingChartImage()
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserID");
            if (!sessionUserId.HasValue)
            {
                byte[] emptyBytes = new byte[0];
                return File(emptyBytes, "image/png");
            }

            int userId = sessionUserId.Value;

            SpendingData spendingData = new SpendingData();
            SpendingSummary summary = spendingData.GetUserSpendingSummary(userId);

            double[] values = new double[4];
            values[0] = (double)summary.HotelsTotal;
            values[1] = (double)summary.FlightsTotal;
            values[2] = (double)summary.CarsTotal;
            values[3] = (double)summary.EventsTotal;

            string[] labels = new string[4];
            labels[0] = "Hotels";
            labels[1] = "Flights";
            labels[2] = "Cars";
            labels[3] = "Events";

            bool allZero = true;
            int index = 0;
            while (index < values.Length)
            {
                if (values[index] > 0.0)
                {
                    allZero = false;
                }

                index = index + 1;
            }

            Plot plt = new Plot();

            if (!allZero)
            {
                List<PieSlice> slices =
                    new List<PieSlice>();

                int i = 0;
                while (i < values.Length)
                {
                    PieSlice slice = new PieSlice();
                    slice.Value = values[i];
                    slice.Label = labels[i];
                    slices.Add(slice);

                    i = i + 1;
                }

                ScottPlot.Plottables.Pie pie = plt.Add.Pie(slices);
                pie.ExplodeFraction = 0.05;
                pie.SliceLabelDistance = 1.3;

                plt.ShowLegend();
                plt.Axes.Frameless();
                plt.HideGrid();
                plt.Title("Your Spending by Category");
            }
            else
            {
                plt.Title("No spending data available yet");
            }

            byte[] bytes = plt.GetImageBytes(600, 400, ScottPlot.ImageFormat.Png);
            return File(bytes, "image/png");
        }
    }
}
