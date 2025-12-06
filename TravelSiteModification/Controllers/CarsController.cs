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
        private readonly DBConnect db = new DBConnect();

        public IActionResult Index()
        {
            CarSearchViewModel model = BuildSearchModel();

            model.PickupLocation = HttpContext.Session.GetString("CarPickupLocation");
            model.DropoffLocation = HttpContext.Session.GetString("CarDropoffLocation");

            model.PickupDate = HttpContext.Session.GetString("CarPickupDate");
            model.DropoffDate = HttpContext.Session.GetString("CarDropoffDate");

            if (string.IsNullOrEmpty(model.PickupLocation) || string.IsNullOrEmpty(model.DropoffLocation))
            {
                model.ErrorMessage = "Please select a pickup and dropoff location.";
                return View(model);
            }

            model.Results = LoadCarsFromDatabase(model.PickupLocation, model.DropoffLocation);
            if (model.Results.Count == 0)
            {
                model.ErrorMessage = "No rental cars were found for your search.";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SearchCars(string pickupLocation, string dropoffLocation, string pickupDate, string dropoffDate)
        {
            HttpContext.Session.SetString("CarPickupLocation", pickupLocation);
            HttpContext.Session.SetString("CarDropoffLocation", dropoffLocation);
            HttpContext.Session.SetString("CarPickupDate", pickupDate);
            HttpContext.Session.SetString("CarDropoffDate", dropoffDate);

            return RedirectToAction("Index");
        }

        private CarSearchViewModel BuildSearchModel()
        {
            CarSearchViewModel model = new CarSearchViewModel();

            model.Locations = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new("New York, NY", "NYC"),
                new("Los Angeles, CA", "LAX"),
                new("Miami, FL", "MIA"),
                new("Seattle, WA", "SEA"),
                new("Philadelphia, PA", "PHL")
            };

            return model;
        }

        private List<CarResultViewModel> LoadCarsFromDatabase(string pickup, string dropoff)
        {
            List<CarResultViewModel> cars = new();

            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "GetCarsByPickupAndDropoff"
            };

            cmd.Parameters.AddWithValue("@PickupLocation", pickup);
            cmd.Parameters.AddWithValue("@DropoffLocation", dropoff);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    cars.Add(new CarResultViewModel
                    {
                        CarID = (int)row["CarID"],
                        AgencyID = (int)row["AgencyID"],
                        CarModel = row["CarModel"].ToString(),
                        CarType = row["CarType"].ToString(),
                        Available = (bool)row["Available"],
                        PricePerDay = (decimal)row["PricePerDay"],
                        ImagePath = row["ImagePath"].ToString()
                    });
                }
            }

            return cars;
        }
        public IActionResult Details(int carId, int agencyId)
        {
            CarDetailViewModel model = new CarDetailViewModel();

            // ---------- 1. LOAD CAR + AGENCY DETAILS ----------
            SqlCommand detailsCmd = new SqlCommand();
            detailsCmd.CommandType = CommandType.StoredProcedure;
            detailsCmd.CommandText = "GetCarAndAgencyDetails";
            detailsCmd.Parameters.AddWithValue("@CarID", carId);

            DataTable carTable = db.GetDataSetUsingCmdObj(detailsCmd).Tables[0];

            if (carTable.Rows.Count == 0)
                return RedirectToAction("Index");

            DataRow row = carTable.Rows[0];

            model.CarID = carId;
            model.AgencyID = agencyId;
            model.CarModel = row["CarModel"].ToString();
            model.CarType = row["CarType"].ToString();
            model.PricePerDay = Convert.ToDecimal(row["PricePerDay"]);
            model.ImagePath = row["ImagePath"].ToString();

            model.AgencyName = row["AgencyName"].ToString();
            model.AgencyPhone = row["Phone"].ToString();
            model.AgencyEmail = row["Email"].ToString();

            // ---------- 2. LOAD OTHER AVAILABLE CARS ----------
            SqlCommand otherCmd = new SqlCommand();
            otherCmd.CommandType = CommandType.StoredProcedure;
            otherCmd.CommandText = "GetOtherAvailableCarsByAgencyID";
            otherCmd.Parameters.AddWithValue("@AgencyID", agencyId);
            otherCmd.Parameters.AddWithValue("@CarID", carId);

            DataSet otherDs = db.GetDataSetUsingCmdObj(otherCmd);

            foreach (DataRow r in otherDs.Tables[0].Rows)
            {
                model.OtherAgencyCars.Add(new CarResultViewModel
                {
                    CarID = (int)r["CarID"],
                    AgencyID = (int)r["AgencyID"],
                    CarModel = r["CarModel"].ToString(),
                    CarType = r["CarType"].ToString(),
                    PricePerDay = Convert.ToDecimal(r["PricePerDay"]),
                    ImagePath = r["ImagePath"].ToString(),
                    Available = true
                });
            }

            return View("Details", model);
        }
    }
}
