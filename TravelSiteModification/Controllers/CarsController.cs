using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

public class CarsController : Controller
{
    private readonly DBConnect db = new DBConnect();

    // -----------------------------
    // MAIN SEARCH PAGE
    // -----------------------------
    public IActionResult Index()
    {
        var pickup = HttpContext.Session.GetString("CarPickupLocation");
        var dropoff = HttpContext.Session.GetString("CarDropoffLocation");
        var pickupDate = HttpContext.Session.GetString("CarPickupDate");
        var dropoffDate = HttpContext.Session.GetString("CarDropoffDate");

        var model = new CarResultsViewModel();

        if (pickup == null || dropoff == null)
        {
            model.Cars = new List<CarResultItem>();
            return View(model);
        }

        model.PickupLocation = pickup;
        model.DropoffLocation = dropoff;

        if (DateTime.TryParse(pickupDate, out var p)) model.PickupDate = p;
        if (DateTime.TryParse(dropoffDate, out var d)) model.DropoffDate = d;

        model.Cars = LoadCars(pickup, dropoff);

        return View(model);
    }

    private List<CarResultItem> LoadCars(string pickup, string dropoff)
    {
        SqlCommand cmd = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "GetCarsByPickupAndDropoff"
        };

        cmd.Parameters.AddWithValue("@PickupLocation", pickup);
        cmd.Parameters.AddWithValue("@DropoffLocation", dropoff);

        DataSet ds = db.GetDataSetUsingCmdObj(cmd);

        List<CarResultItem> list = new();

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            list.Add(new CarResultItem
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

        return list;
    }

    // -----------------------------
    // VIEW DETAILS
    // -----------------------------
    public IActionResult Details(int carId, int agencyId)
    {
        HttpContext.Session.SetInt32("SelectedCarID", carId);

        var model = new CarResultsViewModel
        {
            IsDetailView = true
        };

        model.CarDetails = LoadCarDetails(carId, agencyId);

        return View("Index", model);
    }

    private CarDetailViewModel LoadCarDetails(int carId, int agencyId)
    {
        var result = new CarDetailViewModel();

        SqlCommand detailsCmd = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "GetCarAndAgencyDetails"
        };

        detailsCmd.Parameters.AddWithValue("@CarID", carId);

        DataTable dt = db.GetDataSetUsingCmdObj(detailsCmd).Tables[0];

        if (dt.Rows.Count > 0)
        {
            var row = dt.Rows[0];
            result.Car = new CarResultItem
            {
                CarID = carId,
                AgencyID = agencyId,
                CarModel = row["CarModel"].ToString(),
                CarType = row["CarType"].ToString(),
                PricePerDay = (decimal)row["PricePerDay"],
                ImagePath = row["ImagePath"].ToString(),
                Available = true
            };

            result.AgencyName = row["AgencyName"].ToString();
            result.AgencyPhone = row["Phone"].ToString();
            result.AgencyEmail = row["Email"].ToString();
        }

        SqlCommand otherCmd = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "GetOtherAvailableCarsByAgencyID"
        };

        otherCmd.Parameters.AddWithValue("@AgencyID", agencyId);
        otherCmd.Parameters.AddWithValue("@CarID", carId);

        DataSet otherDS = db.GetDataSetUsingCmdObj(otherCmd);
        foreach (DataRow row in otherDS.Tables[0].Rows)
        {
            result.OtherCars.Add(new CarResultItem
            {
                CarID = (int)row["CarID"],
                AgencyID = (int)row["AgencyID"],
                CarModel = row["CarModel"].ToString(),
                CarType = row["CarType"].ToString(),
                PricePerDay = (decimal)row["PricePerDay"],
                ImagePath = row["ImagePath"].ToString(),
                Available = true
            });
        }

        return result;
    }

    // -----------------------------
    // UPDATE SEARCH
    // -----------------------------
    [HttpPost]
    public IActionResult UpdateSearch(CarResultsViewModel model)
    {
        // Validate
        if (string.IsNullOrEmpty(model.PickupLocation) ||
            string.IsNullOrEmpty(model.DropoffLocation) ||
            model.DropoffDate <= model.PickupDate)
        {
            ModelState.AddModelError("", "Please enter valid locations and dates.");
            return View("Index", model);
        }

        // Save updated criteria to session
        HttpContext.Session.SetString("CarPickupLocation", model.PickupLocation);
        HttpContext.Session.SetString("CarDropoffLocation", model.DropoffLocation);
        HttpContext.Session.SetString("CarPickupDate", model.PickupDate.ToString("yyyy-MM-dd"));
        HttpContext.Session.SetString("CarDropoffDate", model.DropoffDate.ToString("yyyy-MM-dd"));

        return RedirectToAction("Index");
    }

    public IActionResult Book()
    {
        return RedirectToAction("Index", "CarBooking");
    }
}
