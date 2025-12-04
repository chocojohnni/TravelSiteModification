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

        public CarsController(CarAPIService api)
        {
            carApi = api;
        }

        public async Task<IActionResult> Index()
        {
            CarSearchViewModel model = BuildBaseSearchModel();

            model.PickupLocation = HttpContext.Session.GetString("CarPickupLocation");
            model.DropoffLocation = HttpContext.Session.GetString("CarDropoffLocation");
            model.PickupDate = HttpContext.Session.GetString("CarPickupDate");
            model.DropoffDate = HttpContext.Session.GetString("CarDropoffDate");

            if (string.IsNullOrEmpty(model.PickupLocation) || string.IsNullOrEmpty(model.DropoffLocation))
            {
                model.ErrorMessage = "No locations selected. Please return to the home page.";
                return View(model);
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CarSearchViewModel model)
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

            DateTime pickup;
            DateTime dropoff;

            bool pickupValid = DateTime.TryParse(model.PickupDate, out pickup);
            bool dropoffValid = DateTime.TryParse(model.DropoffDate, out dropoff);

            if (pickupValid && dropoffValid)
            {
                if (dropoff <= pickup)
                {
                    newModel.ErrorMessage = "Dropoff date must be after the pickup date.";
                    return View(newModel);
                }
            }

            HttpContext.Session.SetString("CarPickupLocation", model.PickupLocation);
            HttpContext.Session.SetString("CarDropoffLocation", model.DropoffLocation);
            HttpContext.Session.SetString("CarPickupDate", model.PickupDate);
            HttpContext.Session.SetString("CarDropoffDate", model.DropoffDate);

            newModel.Results = await LoadCarsFromApi(model.PickupLocation, model.DropoffLocation);

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

        public async Task<IActionResult> Details(int carId, int agencyId)
        {
            string pickupCode = HttpContext.Session.GetString("CarPickupLocation");
            string dropoffCode = HttpContext.Session.GetString("CarDropoffLocation");

            CarDetailViewModel model = await LoadCarDetailsFromApi(carId, agencyId, pickupCode, dropoffCode);

            if (model == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("SelectedCarID", carId);

            return View(model);
        }

        private async Task<List<CarResultViewModel>> LoadCarsFromApi(string pickup, string dropoff)
        {
            List<CarResultViewModel> list = new List<CarResultViewModel>();

            List<CarAPIModel> raw = await carApi.FindCarsAsync(pickup, dropoff, "", 0, 10000);

            if (raw == null || raw.Count == 0)
            {
                return list;
            }

            int i = 0;
            while (i < raw.Count)
            {
                CarAPIModel api = raw[i];
                CarResultViewModel car = new CarResultViewModel();

                car.CarID = api.CarID;
                car.AgencyID = api.AgencyID;
                car.CarModel = api.CarModel;
                car.CarType = api.CarType;
                car.PricePerDay = api.DailyRate;
                car.Available = api.Available;
                car.ImagePath = api.ImagePath;

                list.Add(car);
                i = i + 1;
            }

            return list;
        }

        private async Task<CarDetailViewModel> LoadCarDetailsFromApi(int carId, int agencyId, string pickup, string dropoff)
        {
            List<CarAPIModel> cars = await carApi.GetCarsByAgencyAsync(agencyId, pickup, dropoff);

            if (cars == null || cars.Count == 0)
            {
                return null;
            }

            int i = 0;
            while (i < cars.Count)
            {
                CarAPIModel c = cars[i];

                if (c.CarID == carId)
                {
                    CarDetailViewModel model = new CarDetailViewModel();
                    model.CarID = c.CarID;
                    model.AgencyID = c.AgencyID;
                    model.CarModel = c.CarModel;
                    model.CarType = c.CarType;
                    model.PricePerDay = c.DailyRate;
                    model.ImagePath = c.ImagePath;

                    model.GalleryImages = new List<string>();

                    if (!string.IsNullOrEmpty(c.ImagePath))
                    {
                        model.GalleryImages.Add(c.ImagePath);

                        string ext = System.IO.Path.GetExtension(c.ImagePath);
                        string prefix = c.ImagePath.Replace(ext, "");

                        model.GalleryImages.Add(prefix + "_2" + ext);
                        model.GalleryImages.Add(prefix + "_3" + ext);
                    }
                    else
                    {
                        model.GalleryImages.Add("/images/cars/default1.jpg");
                        model.GalleryImages.Add("/images/cars/default2.jpg");
                        model.GalleryImages.Add("/images/cars/default3.jpg");
                    }

                    model.OtherAgencyCars = new List<CarResultViewModel>();

                    int j = 0;
                    while (j < cars.Count)
                    {
                        CarAPIModel oc = cars[j];

                        if (oc.CarID != carId)
                        {
                            CarResultViewModel item = new CarResultViewModel();
                            item.CarID = oc.CarID;
                            item.AgencyID = oc.AgencyID;
                            item.CarModel = oc.CarModel;
                            item.CarType = oc.CarType;
                            item.PricePerDay = oc.DailyRate;
                            item.Available = oc.Available;
                            item.ImagePath = oc.ImagePath;

                            model.OtherAgencyCars.Add(item);
                        }

                        j = j + 1;
                    }

                    return model;
                }

                i = i + 1;
            }

            return null;
        }

        private string FormatCity(string code)
        {
            if (code == "NYC") return "New York, NY";
            if (code == "LAX") return "Los Angeles, CA";
            if (code == "MIA") return "Miami, FL";
            if (code == "SEA") return "Seattle, WA";
            if (code == "PHL") return "Philadelphia, PA";
            return code;
        }

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

            item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
            item.Value = "PHL";
            item.Text = "Philadelphia, PA";
            locations.Add(item);

            model.Locations = locations;

            return model;
        }
    }
}
