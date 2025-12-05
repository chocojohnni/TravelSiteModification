using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> Index()
        {
            CarSearchViewModel model = BuildSearchModel();

            model.PickupLocation = HttpContext.Session.GetString("CarPickupLocation");
            model.DropoffLocation = HttpContext.Session.GetString("CarDropoffLocation");
            model.PickupDate = HttpContext.Session.GetString("CarPickupDate");
            model.DropoffDate = HttpContext.Session.GetString("CarDropoffDate");

            if (string.IsNullOrEmpty(model.PickupLocation))
            {
                model.ErrorMessage = "No pickup location selected.";
                return View(model);
            }

            // Load cars using API
            model.Results = await LoadCarsFromApi(model.PickupLocation);

            if (model.Results == null || model.Results.Count == 0)
                model.ErrorMessage = "No rental cars were found.";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="", Text="-- Select City --"},
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="NYC", Text="New York, NY"},
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="LAX", Text="Los Angeles, CA"},
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="MIA", Text="Miami, FL"},
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="SEA", Text="Seattle, WA"},
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value="PHL", Text="Philadelphia, PA"}
            };

            return model;
        }

        private async Task<List<CarResultViewModel>> LoadCarsFromApi(string pickupCode)
        {
            string city = pickupCode;
            
            List<CarAPIModel> raw = await carApi.FindCarsAsync(city, "", 0, 10000);

            List<CarResultViewModel> result = new List<CarResultViewModel>();

            foreach (var car in raw)
            {
                result.Add(MapApiCarToResult(car));
            }

            return result;
        }

        private string ConvertCodeToCityName(string code)
        {
            return code switch
            {
                "NYC" => "New York",
                "LAX" => "Los Angeles",
                "MIA" => "Miami",
                "SEA" => "Seattle",
                "PHL" => "Philadelphia",
                _ => code
            };
        }

        private CarResultViewModel MapApiCarToResult(CarAPIModel apiCar)
        {
            return new CarResultViewModel
            {
                CarID = apiCar.CarID,
                AgencyID = apiCar.AgencyID,
                CarModel = apiCar.CarModel,
                CarType = apiCar.CarType,
                PricePerDay = apiCar.DailyRate, // mapping handled correctly
                Available = apiCar.Available,
                ImagePath = apiCar.ImagePath
            };
        }
    }
}
