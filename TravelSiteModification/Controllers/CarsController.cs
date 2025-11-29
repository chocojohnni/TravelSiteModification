using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

public class CarsController : Controller
{
    private readonly DBConnect db = new DBConnect();
    public IActionResult Index()
    {
        var model = new CarSearchViewModel
        {
            PickupLocation = "",
            DropoffLocation = "",
            PickupDate = DateTime.Today.ToString("yyyy-MM-dd"),
            DropoffDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),

            // Prevent null reference
            Locations = new List<string> { "SEA", "LAX", "NYC", "MIA" },
            Results = new List<Car>(),

            Message = "",
            ErrorMessage = ""
        };

        return View("Cars", model);
    }


    [HttpPost]
    public IActionResult SearchCars(string pickupLocation, string dropoffLocation, string pickupDate, string dropoffDate)
    {
        // Save search inputs to session
        HttpContext.Session.SetString("CarPickupLocation", pickupLocation);
        HttpContext.Session.SetString("CarDropoffLocation", dropoffLocation);
        HttpContext.Session.SetString("CarPickupDate", pickupDate);
        HttpContext.Session.SetString("CarDropoffDate", dropoffDate);

        // Go to results
        return RedirectToAction("Results");
    }

    public IActionResult Results()
    {
        // Get search criteria from session
        var pickup = HttpContext.Session.GetString("CarPickupLocation");
        var dropoff = HttpContext.Session.GetString("CarDropoffLocation");
        var pickupDate = HttpContext.Session.GetString("CarPickupDate");
        var dropoffDate = HttpContext.Session.GetString("CarDropoffDate");

        // Protect from direct navigation
        if (pickup == null || dropoff == null)
            return RedirectToAction("Index");

        var model = new CarSearchViewModel
        {
            PickupLocation = pickup,
            DropoffLocation = dropoff,
            PickupDate = DateTime.Parse(pickupDate),
            DropoffDate = DateTime.Parse(dropoffDate),
            Results = new List<CarResultViewModel>()
        };

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GetCarsByPickupAndDropoff";
        cmd.Parameters.AddWithValue("@PickupLocation", pickup);
        cmd.Parameters.AddWithValue("@DropoffLocation", dropoff);

        DataSet ds = db.GetDataSetUsingCmdObj(cmd);

        if (ds.Tables.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                model.Results.Add(new CarResultViewModel
                {
                    CarID = Convert.ToInt32(row["CarID"]),
                    AgencyID = Convert.ToInt32(row["AgencyID"]),
                    CarModel = row["CarModel"].ToString(),
                    CarType = row["CarType"].ToString(),
                    PricePerDay = Convert.ToDecimal(row["PricePerDay"]),
                    ImagePath = row["ImagePath"].ToString()
                });
            }
        }
        return View(model);
    }

    public IActionResult Details(int carId, int agencyId)
    {
        var model = new CarDetailViewModel();
        DBConnect db = new DBConnect();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GetCarAndAgencyDetails";
        cmd.Parameters.AddWithValue("@CarID", carId);

        DataSet ds = db.GetDataSetUsingCmdObj(cmd);

        if (ds.Tables[0].Rows.Count == 0)
            return RedirectToAction("Index");

        DataRow row = ds.Tables[0].Rows[0];

        model.CarID = carId;
        model.CarModel = row["CarModel"].ToString();
        model.CarType = row["CarType"].ToString();
        model.PricePerDay = Convert.ToDecimal(row["PricePerDay"]);
        model.ImagePath = row["ImagePath"].ToString();

        model.AgencyName = row["AgencyName"].ToString();
        model.AgencyPhone = row["Phone"].ToString();
        model.AgencyEmail = row["Email"].ToString();

        // Get "other cars from agency"
        SqlCommand cmd2 = new SqlCommand();
        cmd2.CommandType = CommandType.StoredProcedure;
        cmd2.CommandText = "GetOtherAvailableCarsByAgencyID";
        cmd2.Parameters.AddWithValue("@AgencyID", agencyId);
        cmd2.Parameters.AddWithValue("@CarID", carId);

        DataSet ds2 = db.GetDataSetUsingCmdObj(cmd2);

        model.OtherCars = new List<AgencyCarViewModel>();

        foreach (DataRow r in ds2.Tables[0].Rows)
        {
            model.OtherCars.Add(new AgencyCarViewModel
            {
                CarID = Convert.ToInt32(r["CarID"]),
                CarModel = r["CarModel"].ToString(),
                CarType = r["CarType"].ToString(),
                PricePerDay = Convert.ToDecimal(r["PricePerDay"]),
                ImagePath = r["ImagePath"].ToString()
            });
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Search(string pickupLocation, string dropoffLocation,
                            string pickupDate, string dropoffDate)
    {
        var model = new CarSearchViewModel
        {
            PickupLocation = pickupLocation,
            DropoffLocation = dropoffLocation,
            PickupDate = pickupDate,
            DropoffDate = dropoffDate,
            Locations = new List<string> { "SEA", "LAX", "NYC", "MIA" }
        };

        try
        {
            model.Results = _repo.SearchCars(model);
            if (!model.Results.Any())
                model.Message = "No search information available.";
        }
        catch (Exception ex)
        {
            model.ErrorMessage = "An error occurred while searching for cars.";
        }

        return View("Cars", model);
    }


    public IActionResult Book(int id)
    {
        return RedirectToAction("Index", "CarBooking", new { carId = id });
    }
}
