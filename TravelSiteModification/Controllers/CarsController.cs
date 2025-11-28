using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

public class CarsController : Controller
{
    private readonly DBConnect db = new DBConnect();

    // -----------------------------
    // SHOW SEARCH + RESULTS
    // -----------------------------
    public IActionResult Index()
    {
        var model = new CarSearchViewModel
        {
            Locations = new List<string> { "SEA", "LAX", "NYC", "MIA" }
        };

        return View(model);
    }

    // -----------------------------
    // POST: PERFORM SEARCH
    // -----------------------------
    [HttpPost]
    public IActionResult Index(CarSearchViewModel model)
    {
        if (string.IsNullOrEmpty(model.PickupLocation) ||
            string.IsNullOrEmpty(model.DropoffLocation))
        {
            model.ErrorMessage = "Please choose valid pickup and dropoff locations.";
            return View(model);
        }

        if (model.DropoffDate <= model.PickupDate)
        {
            model.ErrorMessage = "Dropoff date must be after pickup date.";
            return View(model);
        }

        SqlCommand cmd = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "GetCarsByPickupAndDropoff"
        };

        cmd.Parameters.AddWithValue("@PickupLocation", model.PickupLocation);
        cmd.Parameters.AddWithValue("@DropoffLocation", model.DropoffLocation);

        DataSet ds = db.GetDataSetUsingCmdObj(cmd);

        model.Results = new List<CarResultViewModel>();

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

        model.Locations = new List<string> { "SEA", "LAX", "NYC", "MIA" };

        return View(model);
    }

    // -----------------------------
    // CAR DETAILS PAGE
    // -----------------------------
    public IActionResult Details(int carId, int agencyId)
    {
        var model = new CarDetailsViewModel();
        DBConnect db = new DBConnect();

        // 1) MAIN CAR DETAILS
        SqlCommand cmd = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "GetCarAndAgencyDetails"
        };
        cmd.Parameters.AddWithValue("@CarID", carId);

        DataTable dt = db.GetDataSetUsingCmdObj(cmd).Tables[0];

        if (dt.Rows.Count == 0)
            return RedirectToAction("Index");

        DataRow row = dt.Rows[0];

        model.CarID = carId;
        model.CarModel = row["CarModel"].ToString();
        model.CarType = row["CarType"].ToString();
        model.PricePerDay = Convert.ToDecimal(row["PricePerDay"]);
        model.ImagePath = row["ImagePath"].ToString();

        model.AgencyName = row["AgencyName"].ToString();
        model.AgencyPhone = row["Phone"].ToString();
        model.AgencyEmail = row["Email"].ToString();

        // 2) OTHER CARS
        SqlCommand cmd2 = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "GetOtherAvailableCarsByAgencyID"
        };
        cmd2.Parameters.AddWithValue("@AgencyID", agencyId);
        cmd2.Parameters.AddWithValue("@CarID", carId);

        DataSet ds = db.GetDataSetUsingCmdObj(cmd2);

        model.OtherCars = new List<AgencyCarViewModel>();

        foreach (DataRow r in ds.Tables[0].Rows)
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

    // -----------------------------
    // BOOK THE CAR
    // -----------------------------
    public IActionResult Book(int id)
    {
        return RedirectToAction("Index", "CarBooking", new { carId = id });
    }
}
