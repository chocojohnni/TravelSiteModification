using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class HotelBookingController : Controller
    {
        // GET: /HotelBooking/Index
        public IActionResult Index()
        {
            int? hotelId = HttpContext.Session.GetInt32("SelectedHotelID");
            int? roomId = HttpContext.Session.GetInt32("SelectedRoomID");

            if (hotelId == null || roomId == null)
            {
                TempData["Error"] = "Missing hotel or room selection.";
                return RedirectToAction("Index", "Hotel");
            }

            string checkIn = HttpContext.Session.GetString("CheckInDate");
            string checkOut = HttpContext.Session.GetString("CheckOutDate");

            var booking = new HotelBookingViewModel();

            try
            {
                DBConnect db = new DBConnect();

                // Retrieve hotel details
                SqlCommand hotelCmd = new SqlCommand();
                hotelCmd.CommandType = CommandType.StoredProcedure;
                hotelCmd.CommandText = "GetHotelsByID";
                hotelCmd.Parameters.AddWithValue("@HotelID", hotelId);

                DataSet hds = db.GetDataSetUsingCmdObj(hotelCmd);
                if (hds.Tables.Count > 0 && hds.Tables[0].Rows.Count > 0)
                {
                    var row = hds.Tables[0].Rows[0];
                    booking.HotelName = row["HotelName"].ToString();
                    booking.HotelAddress = row["Address"].ToString();
                    booking.HotelPhone = row["Phone"].ToString();
                    booking.HotelEmail = row["Email"].ToString();
                }

                // Retrieve room details
                SqlCommand roomCmd = new SqlCommand();
                roomCmd.CommandType = CommandType.StoredProcedure;
                roomCmd.CommandText = "GetRoomByID";  
                roomCmd.Parameters.AddWithValue("@RoomID", roomId);

                DataSet rds = db.GetDataSetUsingCmdObj(roomCmd);
                if (rds.Tables.Count > 0 && rds.Tables[0].Rows.Count > 0)
                {
                    var row = rds.Tables[0].Rows[0];
                    booking.RoomType = row["RoomType"].ToString();
                    booking.PricePerNight = Convert.ToDecimal(row["Price"]);
                }

                booking.CheckInDate = checkIn;
                booking.CheckOutDate = checkOut;

                // Calculate total nights
                if (DateTime.TryParse(checkIn, out DateTime ci) &&
                    DateTime.TryParse(checkOut, out DateTime co))
                {
                    booking.TotalNights = (co - ci).Days;
                    booking.TotalPrice = booking.TotalNights * booking.PricePerNight;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading booking info: " + ex.Message;
                return RedirectToAction("Index", "Hotel");
            }

            return View(booking);
        }

        // POST: HotelBooking/Confirm
        [HttpPost]
        public IActionResult ConfirmBooking(HotelBookingViewModel model)
        {
            int? hotelId = HttpContext.Session.GetInt32("SelectedHotelID");
            int? roomId = HttpContext.Session.GetInt32("SelectedRoomID");
            string checkIn = HttpContext.Session.GetString("CheckInDate");
            string checkOut = HttpContext.Session.GetString("CheckOutDate");

            // Make sure we have the required session info
            if (hotelId == null || roomId == null || string.IsNullOrEmpty(checkIn) || string.IsNullOrEmpty(checkOut))
            {
                TempData["Error"] = "Missing booking information.";
                return RedirectToAction("Index", "Hotel");
            }

            // Make sure the user is logged in (needed for vacation package)
            int? userIdNullable = HttpContext.Session.GetInt32("UserID");
            if (!userIdNullable.HasValue)
            {
                HttpContext.Session.SetString("RedirectAfterLogin", "HotelBooking");
                return RedirectToAction("Login", "Account");
            }

            int userId = userIdNullable.Value;

            try
            {
                DBConnect db = new DBConnect();

                // Insert the hotel booking
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertHotelBooking";
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@HotelID", hotelId.Value);
                cmd.Parameters.AddWithValue("@RoomID", roomId.Value);
                cmd.Parameters.AddWithValue("@CheckInDate", checkIn);
                cmd.Parameters.AddWithValue("@CheckOutDate", checkOut);
                cmd.Parameters.AddWithValue("@TotalAmount", model.TotalPrice);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName ?? string.Empty);
                cmd.Parameters.AddWithValue("@LastName", model.LastName ?? string.Empty);
                cmd.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);

                int result = db.DoUpdateUsingCmdObj(cmd);

                if (result > 0)
                {
                    // Add this hotel to package
                    try
                    {
                        decimal costToAdd = model.TotalPrice;

                        int packageId = GetOrCreateOpenVacationPackage(userId, costToAdd);

                        SqlCommand pkgCmd = new SqlCommand();
                        pkgCmd.CommandType = CommandType.StoredProcedure;
                        pkgCmd.CommandText = "InsertPackageHotel";
                        pkgCmd.Parameters.AddWithValue("@PackageID", packageId);
                        pkgCmd.Parameters.AddWithValue("@HotelID", hotelId.Value);
                        pkgCmd.Parameters.AddWithValue("@RoomID", roomId.Value);

                        db.DoUpdateUsingCmdObj(pkgCmd);

                        TempData["Success"] = "Booking confirmed and added to your vacation package!";
                    }
                    catch (Exception pkgEx)
                    {
                        TempData["Success"] =
                            "Booking confirmed, but the hotel could not be added to your vacation package: "
                            + pkgEx.Message;
                    }
                    HttpContext.Session.Remove("SelectedHotelID");
                    HttpContext.Session.Remove("SelectedRoomID");
                    HttpContext.Session.Remove("RoomPrice");
                    HttpContext.Session.Remove("CheckInDate");
                    HttpContext.Session.Remove("CheckOutDate");

                    return RedirectToAction("Receipt");
                }
                else
                {
                    TempData["Error"] = "Booking failed.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Booking error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SelectRoom(int roomId, string checkInDate, string checkOutDate)
        {
            // Store selected room + dates in session
            HttpContext.Session.SetInt32("SelectedRoomID", roomId);
            HttpContext.Session.SetString("CheckInDate", checkInDate);
            HttpContext.Session.SetString("CheckOutDate", checkOutDate);

            // Redirect user to the booking page
            return RedirectToAction("Index", "HotelBooking");
        }


        // GET: HotelBooking/Receipt
        public IActionResult Receipt()
        {
            return View();
        }

        private int GetOrCreateOpenVacationPackage(int userId, decimal additionalCost)
        {
            int packageId = 0;

            int? sessionPackageId = HttpContext.Session.GetInt32("CurrentPackageID");
            if (sessionPackageId.HasValue)
            {
                packageId = sessionPackageId.Value;
            }

            DBConnect dbConnect = new DBConnect();

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
                findCmd.Parameters.AddWithValue("@Status", "Building");

                DataSet ds = dbConnect.GetDataSetUsingCmdObj(findCmd);

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
                insertCmd.Parameters.AddWithValue("@Status", "Building");

                SqlParameter outputParam = new SqlParameter("@NewPackageID", System.Data.SqlDbType.Int);
                outputParam.Direction = System.Data.ParameterDirection.Output;
                insertCmd.Parameters.Add(outputParam);

                dbConnect.DoUpdateUsingCmdObj(insertCmd);

                packageId = Convert.ToInt32(outputParam.Value);
            }
            else
            {
                SqlCommand updateCmd = new SqlCommand();
                updateCmd.CommandType = CommandType.Text;
                updateCmd.CommandText =
                    "UPDATE VacationPackage " +
                    "SET TotalCost = TotalCost + @Amount " +
                    "WHERE PackageID = @PackageID";

                updateCmd.Parameters.AddWithValue("@Amount", additionalCost);
                updateCmd.Parameters.AddWithValue("@PackageID", packageId);

                dbConnect.DoUpdateUsingCmdObj(updateCmd);
            }
            HttpContext.Session.SetInt32("CurrentPackageID", packageId);
            return packageId;
        }
    }
}

