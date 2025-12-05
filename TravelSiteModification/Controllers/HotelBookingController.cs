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

            HotelBookingViewModel booking = new HotelBookingViewModel();

            try
            {
                DBConnect db = new DBConnect();

                SqlCommand hotelCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "GetHotelsByID"
                };
                hotelCmd.Parameters.AddWithValue("@HotelID", hotelId.Value);

                DataSet hds = db.GetDataSetUsingCmdObj(hotelCmd);

                if (hds.Tables.Count > 0 && hds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = hds.Tables[0].Rows[0];

                    booking.HotelID = hotelId.Value;
                    booking.HotelName = row["HotelName"].ToString();
                    booking.HotelAddress = row["Address"].ToString();
                    booking.HotelPhone = row["Phone"].ToString();
                    booking.HotelEmail = row["Email"].ToString();
                }

                SqlCommand roomCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "GetRoomByID"
                };
                roomCmd.Parameters.AddWithValue("@RoomID", roomId.Value);

                DataSet rds = db.GetDataSetUsingCmdObj(roomCmd);

                if (rds.Tables.Count > 0 && rds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = rds.Tables[0].Rows[0];

                    booking.RoomID = roomId.Value;
                    booking.RoomType = row["RoomType"].ToString();
                    booking.PricePerNight = Convert.ToDecimal(row["Price"]);
                }

                booking.CheckInDate = checkIn;
                booking.CheckOutDate = checkOut;

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

        [HttpPost]
        public IActionResult SelectRoom(int hotelId, int roomId, string checkInDate, string checkOutDate)
        {
            HttpContext.Session.SetInt32("SelectedHotelID", hotelId);
            HttpContext.Session.SetInt32("SelectedRoomID", roomId);
            HttpContext.Session.SetString("CheckInDate", checkInDate);
            HttpContext.Session.SetString("CheckOutDate", checkOutDate);

            return RedirectToAction("Index"); // Goes to HotelBooking/Index
        }

        [HttpPost]
        public IActionResult ConfirmBooking(HotelBookingViewModel model)
        {
            int? hotelId = HttpContext.Session.GetInt32("SelectedHotelID");
            int? roomId = HttpContext.Session.GetInt32("SelectedRoomID");
            string checkIn = HttpContext.Session.GetString("CheckInDate");
            string checkOut = HttpContext.Session.GetString("CheckOutDate");

            if (hotelId == null || roomId == null || checkIn == null || checkOut == null)
            {
                TempData["Error"] = "Missing booking information.";
                return RedirectToAction("Index", "Hotel");
            }

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

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "AddHotelBooking"
                };

                cmd.Parameters.AddWithValue("@HotelID", hotelId.Value);
                cmd.Parameters.AddWithValue("@RoomID", roomId.Value);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName ?? "");
                cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@TotalPrice", model.TotalPrice);

                DateTime ci = DateTime.Parse(checkIn);
                DateTime co = DateTime.Parse(checkOut);
                cmd.Parameters.AddWithValue("@CheckInDate", ci);
                cmd.Parameters.AddWithValue("@CheckOutDate", co);

                cmd.Parameters.AddWithValue("@Status", "Confirmed");
                cmd.Parameters.AddWithValue("@UserID", userId);

                int result = db.DoUpdateUsingCmdObj(cmd);

                if (result > 0)
                {
                    // Add to vacation package system
                    int packageId = GetOrCreateOpenVacationPackage(userId, model.TotalPrice);

                    SqlCommand pkgCmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "InsertPackageHotel"
                    };
                    pkgCmd.Parameters.AddWithValue("@PackageID", packageId);
                    pkgCmd.Parameters.AddWithValue("@HotelID", hotelId.Value);
                    pkgCmd.Parameters.AddWithValue("@RoomID", roomId.Value);

                    db.DoUpdateUsingCmdObj(pkgCmd);

                    TempData["Success"] = "Booking confirmed and added to your vacation package!";
                }
                else
                {
                    TempData["Error"] = "Booking failed.";
                    return RedirectToAction("Index");
                }

                // Clear session
                HttpContext.Session.Remove("SelectedHotelID");
                HttpContext.Session.Remove("SelectedRoomID");
                HttpContext.Session.Remove("CheckInDate");
                HttpContext.Session.Remove("CheckOutDate");

                return RedirectToAction("Receipt");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Booking error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private int GetOrCreateOpenVacationPackage(int userId, decimal additionalCost)
        {
            int packageId = HttpContext.Session.GetInt32("CurrentPackageID") ?? 0;

            DBConnect db = new DBConnect();

            // --- FIND EXISTING OPEN PACKAGE ---
            if (packageId == 0)
            {
                SqlCommand findCmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText =
                    @"SELECT TOP 1 PackageID 
                      FROM VacationPackage 
                      WHERE UserID = @UserID AND Status = 'Open'
                      ORDER BY DateCreated DESC"
                };

                findCmd.Parameters.AddWithValue("@UserID", userId);

                DataSet ds = db.GetDataSetUsingCmdObj(findCmd);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    packageId = Convert.ToInt32(ds.Tables[0].Rows[0]["PackageID"]);
                }
            }

            // --- CREATE NEW PACKAGE IF NONE FOUND ---
            if (packageId == 0)
            {
                SqlCommand insertCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "InsertVacationPackage"
                };

                insertCmd.Parameters.AddWithValue("@UserID", userId);
                insertCmd.Parameters.AddWithValue("@PackageName", "My Vacation Package");
                insertCmd.Parameters.AddWithValue("@StartDate", DateTime.Today);
                insertCmd.Parameters.AddWithValue("@EndDate", DateTime.Today.AddDays(7));
                insertCmd.Parameters.AddWithValue("@TotalCost", additionalCost);
                insertCmd.Parameters.AddWithValue("@Status", "Open");

                SqlParameter outputParam = new SqlParameter("@NewPackageID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                insertCmd.Parameters.Add(outputParam);

                db.DoUpdateUsingCmdObj(insertCmd);

                packageId = Convert.ToInt32(outputParam.Value);
            }
            else
            {
                // --- UPDATE EXISTING PACKAGE ---
                SqlCommand updateCmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText =
                    @"UPDATE VacationPackage 
                      SET TotalCost = TotalCost + @Amount 
                      WHERE PackageID = @PackageID"
                };

                updateCmd.Parameters.AddWithValue("@Amount", additionalCost);
                updateCmd.Parameters.AddWithValue("@PackageID", packageId);

                db.DoUpdateUsingCmdObj(updateCmd);
            }

            HttpContext.Session.SetInt32("CurrentPackageID", packageId);
            return packageId;
        }

        public IActionResult Receipt()
        {
            return View();
        }
    }
}
