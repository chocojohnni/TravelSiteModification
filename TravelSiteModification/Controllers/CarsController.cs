using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelSiteModification.Models;
using TravelSiteModification.Services;

namespace TravelSiteModification.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarAPIService carApi;

        public CarsController(CarAPIService apiService)
        {
            carApi = apiService;
        }

        // GET /Cars
        public async Task<IActionResult> Index()
        {
            CarSearchViewModel model = BuildBaseSearchModel();

            // Load session values
            model.PickupLocation = HttpContext.Session.GetString("CarPickupLocation");
            model.DropoffLocation = HttpContext.Session.GetString("CarDropoffLocation");
            model.PickupDate = HttpContext.Session.GetString("CarPickupDate");
            model.DropoffDate = HttpContext.Session.GetString("CarDropoffDate");

            if (string.IsNullOrEmpty(model.PickupLocation) ||
                string.IsNullOrEmpty(model.DropoffLocation))
            {
                model.ErrorMessage = "No locations selected. Please return to the home page.";
                return View(model);
            }

            // API call - FindCars
            model.Results = await LoadCarsFromApi(model.PickupLocation, model.DropoffLocation);

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

        // POST /Cars (refine search)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CarSearchViewModel model)
        {
            CarSearchViewModel newModel = BuildBaseSearchModel();

            newModel.PickupLocation = model.PickupLocation;
            newModel.DropoffLocation = model.DropoffLocation;
            newModel.PickupDate = model.PickupDate;
            newModel.DropoffDate = model.DropoffDate;

            // Validation
            if (string.IsNullOrEmpty(newModel.PickupLocation) ||
                string.IsNullOrEmpty(newModel.DropoffLocation) ||
                string.IsNullOrEmpty(newModel.PickupDate) ||
                string.IsNullOrEmpty(newModel.DropoffDate))
            {
                newModel.ErrorMessage = "Please select valid locations and dates.";
                return View(newModel);
            }

            DateTime pickup, dropoff;
            bool pickupValid = DateTime.TryParse(newModel.PickupDate, out pickup);
            bool dropoffValid = DateTime.TryParse(newModel.DropoffDate, out dropoff);

            if (pickupValid && dropoffValid)
            {
                if (dropoff <= pickup)
                {
                    newModel.ErrorMessage = "Dropoff date must be after pickup date.";
                    return View(newModel);
                }
            }

            // Save in session
            HttpContext.Session.SetString("CarPickupLocation", newModel.PickupLocation);
            HttpContext.Session.SetString("CarDropoffLocation", newModel.DropoffLocation);
            HttpContext.Session.SetString("CarPickupDate", newModel.PickupDate);
            HttpContext.Session.SetString("CarDropoffDate", newModel.DropoffDate);

            // API call
            newModel.Results = await LoadCarsFromApi(newModel.PickupLocation, newModel.DropoffLocation);

            newModel.SearchCriteriaMessage =
                "Showing results for car rentals from <strong>" +
                FormatCity(newModel.PickupLocation) +
                "</strong> to <strong>" +
                FormatCity(newModel.DropoffLocation) +
                "</strong>";

            if (newModel.Results == null || newModel.Results.Count == 0)
            {
                newModel.ErrorMessage = "No rental cars were found matching your criteria.";
            }

            return View(newModel);
        }

        // GET /Cars/Details
        public async Task<IActionResult> Details(int carId, int agencyId)
        {
            CarDetailViewModel? model = await LoadCarDetail(carId, agencyId);

            if (model == null)
                return NotFound();

            HttpContext.Session.SetInt32("SelectedCarID", carId);

            return View(model);
        }

        // ================================
        // API HELPERS
        // ================================
        private async Task<List<CarResultViewModel>> LoadCarsFromApi(string pickup, string dropoff)
        {
            List<CarResultViewModel> result = new List<CarResultViewModel>();

            // API uses City instead of airport code
            string cityName = ConvertToApiCity(pickup);

            List<CarApiModel> raw = await carApi.FindCarsAsync(cityName, "", 0, 1000);

            int i = 0;
            while (i < raw.Count)
            {
                CarResultViewModel car = new CarResultViewModel();
                car.CarID = raw[i].CarID;
                car.AgencyID = raw[i].AgencyID;
                car.CarModel = raw[i].CarModel;
                car.CarType = raw[i].CarType;
                car.PricePerDay = raw[i].DailyRate;
                car.Available = raw[i].Available;
                car.ImagePath = raw[i].ImagePath;

                result.Add(car);
                i++;
            }

            return result;
        }

        private async Task<CarDetailViewModel?> LoadCarDetail(int carId, int agencyId)
        {
            // API call
            List<CarApiModel> cars =
                await carApi.GetCarsByAgencyAsync(agencyId, "Philadelphia");

            CarApiModel? selected = null;
            int idx = 0;

            while (idx < cars.Count)
            {
                if (cars[idx].CarID == carId)
                {
                    selected = cars[idx];
                    break;
                }
                idx++;
            }

            if (selected == null)
                return null;

            CarDetailViewModel model = new CarDetailViewModel();
            model.CarID = carId;
            model.AgencyID = agencyId;
            model.CarModel = selected.CarModel;
            model.CarType = selected.CarType;
            model.PricePerDay = selected.DailyRate;
            model.ImagePath = selected.ImagePath;

            model.AgencyName = "";
            model.AgencyPhone = "";
            model.AgencyEmail = "";
            model.GalleryImages = new List<string>();

            if (!string.IsNullOrEmpty(selected.ImagePath))
            {
                model.GalleryImages.Add(selected.ImagePath);

                string ext = System.IO.Path.GetExtension(selected.ImagePath);
                string prefix = selected.ImagePath.Replace(ext, "");

                model.GalleryImages.Add(prefix + "_2" + ext);
                model.GalleryImages.Add(prefix + "_3" + ext);
            }

            model.OtherAgencyCars = new List<CarResultViewModel>();

            int j = 0;
            while (j < cars.Count)
            {
                if (cars[j].CarID != carId)
                {
                    CarResultViewModel c = new CarResultViewModel();
                    c.CarID = cars[j].CarID;
                    c.AgencyID = cars[j].AgencyID;
                    c.CarModel = cars[j].CarModel;
                    c.CarType = cars[j].CarType;
                    c.PricePerDay = cars[j].DailyRate;
                    c.Available = cars[j].Available;
                    c.ImagePath = cars[j].ImagePath;

                    model.OtherAgencyCars.Add(c);
                }
                j++;
            }

            return model;
        }

        // ================================
        // UTILITY HELPERS
        // ================================
        private string ConvertToApiCity(string code)
        {
            if (code == "NYC") return "New York";
            if (code == "LAX") return "Los Angeles";
            if (code == "MIA") return "Miami";
            if (code == "SEA") return "Seattle";
            return code;
        }

        private CarSearchViewModel BuildBaseSearchModel()
        {
            CarSearchViewModel model = new CarSearchViewModel();
            var list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "-- Select City --" });
            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "NYC", Text = "New York, NY" });
            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "LAX", Text = "Los Angeles, CA" });
            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "MIA", Text = "Miami, FL" });
            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "SEA", Text = "Seattle, WA" });

            model.Locations = list;

            return model;
        }

        private string FormatCity(string code)
        {
            return ConvertToApiCity(code);
        }
    }
}
