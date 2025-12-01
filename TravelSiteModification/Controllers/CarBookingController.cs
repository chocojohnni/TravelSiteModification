using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using Utilities;
using TravelSiteModification.Models;

namespace TravelSiteModification.Controllers
{
    public class CarBookingController : Controller
    {
        // GET: /CarBooking
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserFirstName") == null)
            {
                HttpContext.Session.SetString("RedirectAfterLogin", "CarBooking");
                return Redirect("/Login.aspx");
            }

            if (HttpContext.Session.GetInt32("SelectedCarID") == null ||
                HttpContext.Session.GetString("CarPickupDate") == null ||
                HttpContext.Session.GetString("CarDropoffDate") == null)
            {
                // go back to car search
                return RedirectToAction("Index", "Cars");
            }

            int selectedCarID = (int)HttpContext.Session.GetInt32("SelectedCarID");
            DateTime pickupDate = Convert.ToDateTime(HttpContext.Session.GetString("CarPickupDate"));
            DateTime dropoffDate = Convert.ToDateTime(HttpContext.Session.GetString("CarDropoffDate"));

            int totalDays = (int)(dropoffDate - pickupDate).TotalDays;
            if (totalDays <= 0)
            {
                totalDays = 1;
            }

            CarBookingViewModel model = BindCarDetails(selectedCarID, pickupDate, dropoffDate, totalDays);

            if (model == null)
            {
                model = new CarBookingViewModel();
                model.StatusMessage = "<p class='alert alert-danger'>Error: Could not retrieve car details.</p>";
            }

            return View(model);
        }

        // POST: /CarBooking
        [HttpPost]
        public IActionResult Index(CarBookingViewModel model)
        {
            if (HttpContext.Session.GetInt32("SelectedCarID") == null ||
                HttpContext.Session.GetString("CarPickupDate") == null ||
                HttpContext.Session.GetString("CarDropoffDate") == null)
            {
                model = new CarBookingViewModel();
                model.StatusMessage =
                    "<p class='alert alert-danger'>Required session data is missing. Please start your car search again.</p>";
                return View(model);
            }

            int selectedCarID = (int)HttpContext.Session.GetInt32("SelectedCarID");
            DateTime pickupDate = Convert.ToDateTime(HttpContext.Session.GetString("CarPickupDate"));
            DateTime dropoffDate = Convert.ToDateTime(HttpContext.Session.GetString("CarDropoffDate"));
            int totalDays = (int)(dropoffDate - pickupDate).TotalDays;
            if (totalDays <= 0)
            {
                totalDays = 1;
            }

            // rebuild car info portion so page still shows details
            CarBookingViewModel baseModel = BindCarDetails(selectedCarID, pickupDate, dropoffDate, totalDays);
            if (baseModel == null)
            {
                baseModel = new CarBookingViewModel();
            }

            // copy driver/payment fields from posted model
            baseModel.FirstName = model.FirstName;
            baseModel.LastName = model.LastName;
            baseModel.Email = model.Email;
            baseModel.CardNumber = model.CardNumber;
            baseModel.ExpiryDate = model.ExpiryDate;
            baseModel.CVV = model.CVV;

            // validation like in WebForms
            if (string.IsNullOrWhiteSpace(baseModel.FirstName) ||
                string.IsNullOrWhiteSpace(baseModel.Email))
            {
                baseModel.StatusMessage =
                    "<p class='text-danger'>Please ensure First Name and Email are filled out.</p>";
                return View(baseModel);
            }

            // final price from Session
            decimal finalPrice;
            if (HttpContext.Session.GetString("CarFinalPrice") != null)
            {
                finalPrice = Convert.ToDecimal(HttpContext.Session.GetString("CarFinalPrice"));
            }
            else
            {
                finalPrice = baseModel.TotalCost;
            }

            string bookingResult = InsertCarBooking(selectedCarID, pickupDate, dropoffDate, finalPrice, baseModel);

            if (bookingResult == "Success")
            {
                baseModel.BookingConfirmed = true;
                baseModel.StatusMessage =
                    "<p class='alert alert-success'><strong>Booking Confirmed!</strong> A confirmation email has been sent.</p>";

                try
                {
                    int? userIdNullable = HttpContext.Session.GetInt32("UserID");
                    if (userIdNullable.HasValue)
                    {
                        int packageId = GetOrCreateOpenVacationPackage(userIdNullable.Value, finalPrice);

                        DBConnect db = new DBConnect();
                        SqlCommand pkgCmd = new SqlCommand();
                        pkgCmd.CommandType = CommandType.StoredProcedure;
                        pkgCmd.CommandText = "spInsertPackageCar";
                        pkgCmd.Parameters.AddWithValue("@PackageID", packageId);
                        pkgCmd.Parameters.AddWithValue("@CarID", selectedCarID);

                        db.DoUpdateUsingCmdObj(pkgCmd);
                    }
                }
                catch (Exception)
                {

                }

                HttpContext.Session.Remove("SelectedCarID");
                HttpContext.Session.Remove("CarFinalPrice");
            }
            else
            {
                baseModel.BookingConfirmed = false;
                baseModel.StatusMessage = bookingResult;
            }

            return View(baseModel);
        }

        // Helper methods
        private CarBookingViewModel BindCarDetails(int selectedCarID, DateTime pickupDate, DateTime dropoffDate, int totalDays)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCarAndAgencyDetails";
            cmd.Parameters.AddWithValue("@CarID", selectedCarID);

            try
            {
                DataSet ds = db.GetDataSetUsingCmdObj(cmd);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    decimal pricePerDay = 0;

                    if (row["PricePerDay"] != DBNull.Value)
                    {
                        pricePerDay = Convert.ToDecimal(row["PricePerDay"]);
                    }

                    decimal finalPrice = pricePerDay * totalDays;

                    CarBookingViewModel model = new CarBookingViewModel();
                    model.SelectedCarID = selectedCarID;

                    model.CarModel = row["CarModel"].ToString();
                    model.CarType = row["CarType"].ToString();
                    model.AgencyName = row["AgencyName"].ToString();
                    model.PickupLocation = row["PickupLocationCode"].ToString();
                    model.DropoffLocation = row["DropoffLocationCode"].ToString();
                    model.PickupDateDisplay = pickupDate.ToShortDateString();
                    model.DropoffDateDisplay = dropoffDate.ToShortDateString();
                    model.TotalDays = totalDays;
                    model.TotalCost = finalPrice;

                    HttpContext.Session.SetString("CarFinalPrice", finalPrice.ToString());

                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                CarBookingViewModel model = new CarBookingViewModel();
                model.StatusMessage = "<p class='alert alert-danger'>Database error: " + ex.Message + "</p>";
                return model;
            }
        }

        private string InsertCarBooking(int selectedCarID, DateTime pickupDate, DateTime dropoffDate, decimal finalPrice, CarBookingViewModel model)
        {
            DBConnect db = new DBConnect();

            int? sessionUserId = HttpContext.Session.GetInt32("UserID");

            if (sessionUserId == null || sessionUserId <= 0)
            {
                return "<p class='alert alert-danger'>❌ User ID is missing or invalid.</p>";
            }

            int userID = sessionUserId.Value;


            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddCarBooking";
            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@CarID", selectedCarID);
            cmd.Parameters.AddWithValue("@PickupDate", pickupDate);
            cmd.Parameters.AddWithValue("@DropoffDate", dropoffDate);
            cmd.Parameters.AddWithValue("@TotalAmount", finalPrice);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? string.Empty);
            cmd.Parameters.AddWithValue("@Email", model.Email);

            try
            {
                int rowsAffected = db.DoUpdateUsingCmdObj(cmd);

                if (rowsAffected > 0)
                {
                    SqlCommand cmdUpdateCar = new SqlCommand();
                    cmdUpdateCar.CommandType = CommandType.StoredProcedure;
                    cmdUpdateCar.CommandText = "UpdateCarAvailability";
                    cmdUpdateCar.Parameters.AddWithValue("@CarID", selectedCarID);

                    db.DoUpdateUsingCmdObj(cmdUpdateCar);

                    return "Success";
                }
                else
                {
                    return "<p class='alert alert-danger'>❌ <strong>Insertion Failed:</strong> No rows were added.</p>";
                }
            }
            catch (Exception ex)
            {
                return "<p class='alert alert-danger'>❌ <strong>Database Exception:</strong> " + ex.Message + "</p>";
            }
        }

        private int GetOrCreateOpenVacationPackage(int userId, decimal additionalCost)
        {
            int packageId = 0;

            int? sessionPackageId = HttpContext.Session.GetInt32("CurrentPackageID");
            if (sessionPackageId.HasValue)
            {
                packageId = sessionPackageId.Value;
            }

            DBConnect db = new DBConnect();

            if (packageId == 0)
            {
                SqlCommand findCmd = new SqlCommand();
                findCmd.CommandType = CommandType.Text;
                findCmd.CommandText =
                    "SELECT TOP 1 PackageID " +
                    "FROM VacationPackage " +
                    "WHERE UserID = @UserID AND Status = @Status " +
                    "ORDER BY DateCreated DESC";

                findCmd.Parameters.AddWithValue("@UserID", userId);
                findCmd.Parameters.AddWithValue("@Status", "Open");

                DataSet ds = db.GetDataSetUsingCmdObj(findCmd);

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    packageId = Convert.ToInt32(ds.Tables[0].Rows[0]["PackageID"]);
                }
            }

            if (packageId == 0)
            {
                SqlCommand insertCmd = new SqlCommand();
                insertCmd.CommandType = CommandType.StoredProcedure;
                insertCmd.CommandText = "InsertVacationPackage";

                insertCmd.Parameters.AddWithValue("@UserID", userId);
                insertCmd.Parameters.AddWithValue("@PackageName", "My Vacation Package");
                insertCmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                insertCmd.Parameters.AddWithValue("@EndDate", DateTime.Today.AddDays(7));
                insertCmd.Parameters.AddWithValue("@TotalCost", additionalCost);
                insertCmd.Parameters.AddWithValue("@Status", "Open");

                SqlParameter outputParam = new SqlParameter("@NewPackageID", System.Data.SqlDbType.Int);
                outputParam.Direction = System.Data.ParameterDirection.Output;
                insertCmd.Parameters.Add(outputParam);

                db.DoUpdateUsingCmdObj(insertCmd);

                packageId = Convert.ToInt32(outputParam.Value);
            }
            else
            {
                SqlCommand updateCmd = new SqlCommand();
                updateCmd.CommandType = CommandType.Text;
                updateCmd.CommandText =
                    "UPDATE VacationPackage " +
                    "SET TotalCost = ISNULL(TotalCost, 0) + @Amount " +
                    "WHERE PackageID = @PackageID";

                updateCmd.Parameters.AddWithValue("@Amount", additionalCost);
                updateCmd.Parameters.AddWithValue("@PackageID", packageId);

                db.DoUpdateUsingCmdObj(updateCmd);
            }

            HttpContext.Session.SetInt32("CurrentPackageID", packageId);

            return packageId;
        }
    }
}
