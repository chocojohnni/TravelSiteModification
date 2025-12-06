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

            // If user previously searched, reload the search results
            if (!string.IsNullOrEmpty(model.PickupLocation) &&
                !string.IsNullOrEmpty(model.DropoffLocation))
            {
                model.Results = LoadCarsFromDatabase(model.PickupLocation, model.DropoffLocation);

                if (model.Results.Count == 0)
                    model.ErrorMessage = "No rental cars were found for your search.";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(CarSearchViewModel model)
        {
            model.Locations = BuildLocationsList();

            // Save search criteria to session
            HttpContext.Session.SetString("CarPickupLocation", model.PickupLocation ?? "");
            HttpContext.Session.SetString("CarDropoffLocation", model.DropoffLocation ?? "");
            HttpContext.Session.SetString("CarPickupDate", model.PickupDate ?? "");
            HttpContext.Session.SetString("CarDropoffDate", model.DropoffDate ?? "");

            // Load results
            model.Results = LoadCarsFromDatabase(model.PickupLocation, model.DropoffLocation);

            if (model.Results.Count == 0)
                model.ErrorMessage = "No cars were found for the selected locations.";

            return View(model);
        }

        public IActionResult Details(int carId, int agencyId)
        {
            CarDetailViewModel model = new CarDetailViewModel();

            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "GetCarAndAgencyDetails"
            };
            cmd.Parameters.AddWithValue("@CarID", carId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                model.CarID = carId;
                model.AgencyID = agencyId;
                model.CarModel = row["CarModel"].ToString();
                model.CarType = row["CarType"].ToString();
                model.PricePerDay = (decimal)row["PricePerDay"];
                model.ImagePath = row["ImagePath"].ToString();

                model.AgencyName = row["AgencyName"].ToString();
                model.AgencyPhone = row["Phone"].ToString();
                model.AgencyEmail = row["Email"].ToString();
            }

            // -------- Load Other Cars From Same Agency --------
            SqlCommand cmd2 = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "GetOtherAvailableCarsByAgencyID"
            };
            cmd2.Parameters.AddWithValue("@AgencyID", agencyId);
            cmd2.Parameters.AddWithValue("@CarID", carId);

            DataSet ds2 = db.GetDataSetUsingCmdObj(cmd2);

            foreach (DataRow row in ds2.Tables[0].Rows)
            {
                model.OtherAgencyCars.Add(new CarResultViewModel
                {
                    CarID = (int)row["CarID"],
                    AgencyID = agencyId,
                    CarModel = row["CarModel"].ToString(),
                    CarType = row["CarType"].ToString(),
                    PricePerDay = (decimal)row["PricePerDay"],
                    ImagePath = row["ImagePath"].ToString(),
                    Available = true
                });
            }

            return View(model);
        }

        public IActionResult Select(int carId)
        {
            HttpContext.Session.SetInt32("SelectedCarID", carId);
            return RedirectToAction("Index", "CarBooking");
        }

        private List<CarResultViewModel> LoadCarsFromDatabase(string pickup, string dropoff)
        {
            List<CarResultViewModel> list = new();

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
                    list.Add(new CarResultViewModel
                    {
                        CarID = (int)row["CarID"],
                        AgencyID = (int)row["AgencyID"],
                        CarModel = row["CarModel"].ToString(),
                        CarType = row["CarType"].ToString(),
                        PricePerDay = (decimal)row["PricePerDay"],
                        ImagePath = row["ImagePath"].ToString(),
                        Available = (bool)row["Available"]
                    });
                }
            }

            return list;
        }

        private CarSearchViewModel BuildSearchModel()
        {
            var model = new CarSearchViewModel();
            model.Locations = BuildLocationsList();
            return model;
        }

        private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> BuildLocationsList()
        {
            return new()
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="", Text="-- Select City --" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="NYC", Text="New York, NY" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="LAX", Text="Los Angeles, CA" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="MIA", Text="Miami, FL" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="SEA", Text="Seattle, WA" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="PHL", Text="Philadelphia, PA" }
            };
        }
    }
}
