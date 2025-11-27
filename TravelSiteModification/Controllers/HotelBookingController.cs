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

            if (hotelId == null || roomId == null)
            {
                TempData["Error"] = "Missing booking information.";
                return RedirectToAction("Index", "Hotel");
            }

            try
            {
                DBConnect db = new DBConnect();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertHotelBooking";

                cmd.Parameters.AddWithValue("@HotelID", hotelId);
                cmd.Parameters.AddWithValue("@RoomID", roomId);
                cmd.Parameters.AddWithValue("@CheckInDate", checkIn);
                cmd.Parameters.AddWithValue("@CheckOutDate", checkOut);
                cmd.Parameters.AddWithValue("@TotalAmount", model.TotalPrice);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@Email", model.Email);

                int result = db.DoUpdateUsingCmdObj(cmd);

                if (result > 0)
                {
                    TempData["Success"] = "Booking confirmed!";
                    return RedirectToAction("Receipt");
                }
                else
                {
                    TempData["Error"] = "Booking failed.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Booking error: " + ex.Message;
            }

            return RedirectToAction("Index");
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
    }
}
