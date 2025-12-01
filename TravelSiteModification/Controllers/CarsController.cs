using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class CarsController : Controller
    {
        // GET: /Cars
        public IActionResult Index()
        {
            CarSearchViewModel model = BuildBaseSearchModel();

            // Load session values set by SearchCars in TravelSiteController
            model.PickupLocation = HttpContext.Session.GetString("CarPickupLocation");
            model.DropoffLocation = HttpContext.Session.GetString("CarDropoffLocation");
            model.PickupDate = HttpContext.Session.GetString("CarPickupDate");
            model.DropoffDate = HttpContext.Session.GetString("CarDropoffDate");

            // Validate
            if (string.IsNullOrEmpty(model.PickupLocation) || string.IsNullOrEmpty(model.DropoffLocation))
            {
                model.ErrorMessage = "No locations selected. Please return to the home page.";
                return View(model);
            }

            // Get cars
            model.Results = GetCarsByPickupAndDropoff(model.PickupLocation, model.DropoffLocation);

            model.SearchCriteriaMessage =
                "Showing results for car rentals from <strong>" +
                FormatCity(model.PickupLocation) +
                "</strong> to <strong>" +
                FormatCity(model.DropoffLocation) +
                "</strong>";

            if (model.Results == null || model.Results.Count == 0)
            {
                model.ErrorMessage = "No rental cars were found matching your criteria.";
            }

            return View(model);
        }

        // POST: Update Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CarSearchViewModel model)
        {
            CarSearchViewModel newModel = BuildBaseSearchModel();

            newModel.PickupLocation = model.PickupLocation;
            newModel.DropoffLocation = model.DropoffLocation;
            newModel.PickupDate = model.PickupDate;
            newModel.DropoffDate = model.DropoffDate;

            if (string.IsNullOrEmpty(model.PickupLocation) ||
                string.IsNullOrEmpty(model.DropoffLocation) ||
                string.IsNullOrEmpty(model.PickupDate) ||
                string.IsNullOrEmpty(model.DropoffDate))
            {
                newModel.ErrorMessage = "Please select valid locations and dates.";
                return View(newModel);
            }

            // Validate date order
            System.DateTime pickup;
            System.DateTime dropoff;

            bool pickupValid = System.DateTime.TryParse(model.PickupDate, out pickup);
            bool dropoffValid = System.DateTime.TryParse(model.DropoffDate, out dropoff);

            if (pickupValid && dropoffValid)
            {
                if (dropoff <= pickup)
                {
                    newModel.ErrorMessage = "Dropoff date must be after the pickup date.";
                    return View(newModel);
                }
            }

            // Save to Session
            HttpContext.Session.SetString("CarPickupLocation", model.PickupLocation);
            HttpContext.Session.SetString("CarDropoffLocation", model.DropoffLocation);
            HttpContext.Session.SetString("CarPickupDate", model.PickupDate);
            HttpContext.Session.SetString("CarDropoffDate", model.DropoffDate);

            // Query
            newModel.Results = GetCarsByPickupAndDropoff(model.PickupLocation, model.DropoffLocation);

            newModel.SearchCriteriaMessage =
                "Showing results for car rentals from <strong>" +
                FormatCity(model.PickupLocation) +
                "</strong> to <strong>" +
                FormatCity(model.DropoffLocation) +
                "</strong>";

            if (newModel.Results == null || newModel.Results.Count == 0)
            {
                newModel.ErrorMessage = "No rental cars were found matching your criteria.";
            }

            return View(newModel);
        }

        // GET: /Cars/Details
        public IActionResult Details(int carId, int agencyId)
        {
            CarDetailViewModel model = GetCarDetail(carId, agencyId);

            if (model == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("SelectedCarID", carId);

            return View(model);
        }

        // Helpers
        private CarSearchViewModel BuildBaseSearchModel()
        {
            CarSearchViewModel model = new CarSearchViewModel();

            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> locations =
                new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem item;

            item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
            item.Value = "";
            item.Text = "-- Select City --";
            locations.Add(item);

            item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
            item.Value = "NYC";
            item.Text = "New York, NY";
            locations.Add(item);

            item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
            item.Value = "LAX";
            item.Text = "Los Angeles, CA";
            locations.Add(item);

            item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
            item.Value = "MIA";
            item.Text = "Miami, FL";
            locations.Add(item);

            item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
            item.Value = "SEA";
            item.Text = "Seattle, WA";
            locations.Add(item);

            model.Locations = locations;

            return model;
        }


        private string FormatCity(string code)
        {
            if (code == "NYC") return "New York, NY";
            if (code == "LAX") return "Los Angeles, CA";
            if (code == "MIA") return "Miami, FL";
            if (code == "SEA") return "Seattle, WA";
            return code;
        }


        private List<CarResultViewModel> GetCarsByPickupAndDropoff(string pickup, string dropoff)
        {
            List<CarResultViewModel> list = new List<CarResultViewModel>();

            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCarsByPickupAndDropoff";
            cmd.Parameters.AddWithValue("@PickupLocation", pickup);
            cmd.Parameters.AddWithValue("@DropoffLocation", dropoff);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable table = ds.Tables[0];
                int i = 0;
                while (i < table.Rows.Count)
                {
                    DataRow row = table.Rows[i];

                    CarResultViewModel car = new CarResultViewModel();
                    car.CarID = int.Parse(row["CarID"].ToString());
                    car.AgencyID = int.Parse(row["AgencyID"].ToString());
                    car.CarModel = row["CarModel"].ToString();
                    car.CarType = row["CarType"].ToString();
                    car.PricePerDay = decimal.Parse(row["PricePerDay"].ToString());
                    car.Available = (bool)row["Available"];
                    car.ImagePath = row["ImagePath"].ToString();

                    list.Add(car);

                    i = i + 1;
                }
            }

            return list;
        }


        private CarDetailViewModel GetCarDetail(int carId, int agencyId)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            CarDetailViewModel model = null;

            // First query — car + agency info
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCarAndAgencyDetails";
            cmd.Parameters.AddWithValue("@CarID", carId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                model = new CarDetailViewModel();
                model.CarID = carId;
                model.AgencyID = agencyId;
                model.CarModel = row["CarModel"].ToString();
                model.CarType = row["CarType"].ToString();
                model.PricePerDay = decimal.Parse(row["PricePerDay"].ToString());
                model.ImagePath = row["ImagePath"].ToString();

                model.AgencyName = row["AgencyName"].ToString();
                model.AgencyPhone = row["Phone"].ToString();
                model.AgencyEmail = row["Email"].ToString();

                model.OtherAgencyCars = new List<CarResultViewModel>();
            }

            if (model == null)
            {
                return null;
            }

            // Second query — other cars From agency
            DBConnect db2 = new DBConnect();
            SqlCommand cmd2 = new SqlCommand();

            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.CommandText = "GetOtherAvailableCarsByAgencyID";
            cmd2.Parameters.AddWithValue("@AgencyID", agencyId);
            cmd2.Parameters.AddWithValue("@CarID", carId);

            DataSet ds2 = db2.GetDataSetUsingCmdObj(cmd2);

            if (ds2 != null && ds2.Tables.Count > 0)
            {
                DataTable table = ds2.Tables[0];
                int i = 0;

                while (i < table.Rows.Count)
                {
                    DataRow row = table.Rows[i];

                    CarResultViewModel car = new CarResultViewModel();
                    car.CarID = int.Parse(row["CarID"].ToString());
                    car.AgencyID = int.Parse(row["AgencyID"].ToString());
                    car.CarModel = row["CarModel"].ToString();
                    car.CarType = row["CarType"].ToString();
                    car.PricePerDay = decimal.Parse(row["PricePerDay"].ToString());
                    car.Available = true; // by definition
                    car.ImagePath = row["ImagePath"].ToString();

                    model.OtherAgencyCars.Add(car);

                    i = i + 1;
                }
            }

            return model;
        }
    }
}
