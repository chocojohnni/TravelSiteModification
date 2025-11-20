using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly DBConnect db;

        public EventController()
        {
            db = new DBConnect();
        }

        [HttpGet]
        public IActionResult Book(int eventId)
        {
            int? userIdNullable = HttpContext.Session.GetInt32("UserID");

            if (!userIdNullable.HasValue)
            {
                string redirectUrl = Url.Action("Book", "Event", new { eventId = eventId });
                HttpContext.Session.SetString("RedirectAfterLogin", redirectUrl);
                return RedirectToAction("Login", "Account");
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEventDetailsByID";
            cmd.Parameters.AddWithValue("@EventID", eventId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Index", "TravelSite");
            }

            DataRow row = ds.Tables[0].Rows[0];

            decimal price = 0;
            if (row["Price"] != DBNull.Value)
            {
                price = Convert.ToDecimal(row["Price"]);
            }

            EventBookingViewModel model = new EventBookingViewModel();
            model.EventId = eventId;
            model.Price = price;

            string firstName = HttpContext.Session.GetString("UserFirstName");
            if (!string.IsNullOrEmpty(firstName))
            {
                model.FirstName = firstName;
            }

            string email = HttpContext.Session.GetString("UserEmail");
            if (!string.IsNullOrEmpty(email))
            {
                model.Email = email;
            }

            ViewBag.EventName = row["EventName"].ToString();
            ViewBag.EventLocation = row["EventLocation"].ToString();
            ViewBag.EventDate = Convert.ToDateTime(row["EventDate"]);
            ViewBag.Description = row["Description"].ToString();
            ViewBag.PricePerTicket = price;
            ViewBag.ImagePath = row["ImagePath"].ToString();

            return View("~/Views/TravelSite/EventBooking.cshtml", model);
        }

        [HttpPost]
        public IActionResult Book(EventBookingViewModel model)
        {
            int? userIdNullable = HttpContext.Session.GetInt32("UserID");
            if (!userIdNullable.HasValue)
            {
                string redirectUrl = Url.Action("Book", "Event", new { eventId = model.EventId });
                HttpContext.Session.SetString("RedirectAfterLogin", redirectUrl);
                return RedirectToAction("Login", "Account");
            }

            // reload event details
            SqlCommand reloadCmd = new SqlCommand();
            reloadCmd.CommandType = CommandType.StoredProcedure;
            reloadCmd.CommandText = "GetEventDetailsByID";
            reloadCmd.Parameters.AddWithValue("@EventID", model.EventId);

            DataSet ds = db.GetDataSetUsingCmdObj(reloadCmd);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                ViewBag.EventName = row["EventName"].ToString();
                ViewBag.EventLocation = row["EventLocation"].ToString();
                ViewBag.EventDate = Convert.ToDateTime(row["EventDate"]);
                ViewBag.Description = row["Description"].ToString();
                ViewBag.ImagePath = row["ImagePath"].ToString();

                if (model.Price == 0 && row["Price"] != DBNull.Value)
                {
                    model.Price = Convert.ToDecimal(row["Price"]);
                }

                ViewBag.PricePerTicket = model.Price;
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/TravelSite/EventBooking.cshtml", model);
            }

            // Calculate Total Price
            decimal totalPrice = model.Price * model.TicketCount;

            SqlCommand insertCmd = new SqlCommand();
            insertCmd.CommandType = CommandType.StoredProcedure;
            insertCmd.CommandText = "AddEventBooking";

            insertCmd.Parameters.AddWithValue("@EventID", model.EventId);
            insertCmd.Parameters.AddWithValue("@UserID", userIdNullable.Value);
            insertCmd.Parameters.AddWithValue("@FirstName", model.FirstName.Trim());
            insertCmd.Parameters.AddWithValue("@LastName", model.LastName.Trim());
            insertCmd.Parameters.AddWithValue("@Email", model.Email.Trim());
            insertCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);
            insertCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
            insertCmd.Parameters.AddWithValue("@Status", "Pending");

            try
            {
                int rows = db.DoUpdateUsingCmdObj(insertCmd);

                if (rows > 0)
                {
                    ViewBag.IsSuccess = true;
                    ViewBag.StatusMessage = "Your event tickets have been booked successfully.";
                }
                else
                {
                    ViewBag.IsSuccess = false;
                    ViewBag.StatusMessage = "There was an issue saving your booking.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsSuccess = false;
                ViewBag.StatusMessage = "Database error: " + ex.Message;
            }

            return View("~/Views/TravelSite/EventBooking.cshtml", model);
        }

    }
}
