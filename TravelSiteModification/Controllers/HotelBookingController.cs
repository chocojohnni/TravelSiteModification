using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class HotelBookingController : Controller
    {
        private readonly DBConnect db;

        public HotelBookingController()
        {
            db = new DBConnect();
        }

        // GET: /HotelBooking?hotelId=3&roomId=12
        [HttpGet]
        public IActionResult Index(int hotelId, int roomId)
        {
            // Require login
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                HttpContext.Session.SetString("RedirectAfterLogin", $"/HotelBooking?hotelId={hotelId}&roomId={roomId}");
                return RedirectToAction("Login", "Account");
            }

            var model = new HotelBookingViewModel
            {
                HotelID = hotelId,
                RoomID = roomId
            };

            // Load booking details
            LoadBookingDetails(model);

            // Load session dates
            model.CheckInDate = HttpContext.Session.GetString("CheckInDate");
            model.CheckOutDate = HttpContext.Session.GetString("CheckOutDate");

            return View(model);
        }

        private void LoadBookingDetails(HotelBookingViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetBookingDetailsByHotelAndRoom";
            cmd.Parameters.AddWithValue("@HotelID", model.HotelID);
            cmd.Parameters.AddWithValue("@RoomID", model.RoomID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return;

            DataRow row = ds.Tables[0].Rows[0];

            model.HotelName = row["HotelName"].ToString();
            model.RoomType = row["RoomType"].ToString();
            model.Capacity = row["Capacity"].ToString();
            model.PricePerNight = Convert.ToDecimal(row["Price"]);
        }

        // POST — insert booking
        [HttpPost]
        public IActionResult Index(HotelBookingViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                model.StatusMessage = "User not logged in.";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.FirstName) ||
                string.IsNullOrWhiteSpace(model.Email))
            {
                model.StatusMessage = "Please enter all required guest information.";
                return View(model);
            }

            string result = InsertBooking(model, userId.Value);

            if (result == "Success")
            {
                model.BookingSuccessful = true;
                model.StatusMessage = "Booking Confirmed! A confirmation email has been sent.";

                // Clear booking data
                HttpContext.Session.Remove("SelectedHotelID");
                HttpContext.Session.Remove("SelectedRoomID");
                HttpContext.Session.Remove("RoomPrice");
            }
            else
            {
                model.StatusMessage = result;
            }

            return View(model);
        }

        private string InsertBooking(HotelBookingViewModel model, int userId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddHotelBooking";

            cmd.Parameters.AddWithValue("@HotelID", model.HotelID);
            cmd.Parameters.AddWithValue("@RoomID", model.RoomID);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@TotalPrice", model.PricePerNight);
            cmd.Parameters.AddWithValue("@Status", "Pending");
            cmd.Parameters.AddWithValue("@UserID", userId);

            if (!string.IsNullOrEmpty(model.CheckInDate))
                cmd.Parameters.AddWithValue("@CheckInDate", Convert.ToDateTime(model.CheckInDate));
            else
                cmd.Parameters.AddWithValue("@CheckInDate", DBNull.Value);

            if (!string.IsNullOrEmpty(model.CheckOutDate))
                cmd.Parameters.AddWithValue("@CheckOutDate", Convert.ToDateTime(model.CheckOutDate));
            else
                cmd.Parameters.AddWithValue("@CheckOutDate", DBNull.Value);

            try
            {
                db.DoUpdateUsingCmdObj(cmd);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Database Error: {ex.Message}";
            }
        }

        // Back button
        public IActionResult Back(int hotelId)
        {
            return RedirectToAction("Details", "Hotel", new { id = hotelId });
        }
    }
}
