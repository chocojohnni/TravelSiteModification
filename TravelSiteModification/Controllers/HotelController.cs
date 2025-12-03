using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class HotelController : Controller
    {
        private readonly DBConnect _db;

        public HotelController()
        {
            _db = new DBConnect();
        }

        [HttpGet]
        public IActionResult Index()
        {
            HotelSearchViewModel model = new HotelSearchViewModel();
            PopulateDestinations(model);

            string destination = HttpContext.Session.GetString("HotelDestination");
            string checkInStr = HttpContext.Session.GetString("CheckInDate");
            string checkOutStr = HttpContext.Session.GetString("CheckOutDate");

            if (!string.IsNullOrEmpty(destination))
            {
                model.Destination = destination;

                DateTime dt;
                if (!string.IsNullOrEmpty(checkInStr) && DateTime.TryParse(checkInStr, out dt))
                {
                    model.CheckInDate = dt;
                }
                if (!string.IsNullOrEmpty(checkOutStr) && DateTime.TryParse(checkOutStr, out dt))
                {
                    model.CheckOutDate = dt;
                }

                LoadHotelResults(model);
            }
            else
            {
                model.ErrorMessage = "No destination selected. Please return to the home page.";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(HotelSearchViewModel model)
        {
            PopulateDestinations(model);

            if (string.IsNullOrEmpty(model.Destination))
            {
                model.ErrorMessage = "Please select a valid destination.";
                model.Results = new List<HotelResultViewModel>();
                return View(model);
            }

            // Save new search values to session
            HttpContext.Session.SetString("HotelDestination", model.Destination ?? "");

            if (model.CheckInDate.HasValue)
            {
                HttpContext.Session.SetString("CheckInDate", model.CheckInDate.Value.ToString("yyyy-MM-dd"));
            }

            if (model.CheckOutDate.HasValue)
            {
                HttpContext.Session.SetString("CheckOutDate", model.CheckOutDate.Value.ToString("yyyy-MM-dd"));
            }

            LoadHotelResults(model);

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int hotelId)
        {
            HotelDetailsViewModel model = new HotelDetailsViewModel();

            LoadHotelDetails(hotelId, model);
            LoadRoomAvailability(hotelId, model);

            // pull dates from session
            string checkInStr = HttpContext.Session.GetString("CheckInDate");
            string checkOutStr = HttpContext.Session.GetString("CheckOutDate");

            DateTime dt;

            if (!string.IsNullOrEmpty(checkInStr) && DateTime.TryParse(checkInStr, out dt))
            {
                model.CheckInDate = dt;
            }
            if (!string.IsNullOrEmpty(checkOutStr) && DateTime.TryParse(checkOutStr, out dt))
            {
                model.CheckOutDate = dt;
            }

            if (model.HotelID == 0)
            {
                TempData["HotelError"] = "Hotel not found.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        private void PopulateDestinations(HotelSearchViewModel model)
        {
            model.Destinations = new List<SelectListItem>();

            model.Destinations.Add(new SelectListItem { Text = "-- Select Destination --", Value = "" });
            model.Destinations.Add(new SelectListItem { Text = "New York, NY", Value = "New York" });
            model.Destinations.Add(new SelectListItem { Text = "Los Angeles, CA", Value = "Los Angeles" });
            model.Destinations.Add(new SelectListItem { Text = "Miami, FL", Value = "Miami" });
            model.Destinations.Add(new SelectListItem { Text = "Seattle, WA", Value = "Seattle" });
        }

        private void LoadHotelResults(HotelSearchViewModel model)
        {
            if (string.IsNullOrEmpty(model.Destination))
            {
                model.ErrorMessage = "No destination selected. Please return to the home page.";
                model.Results = new List<HotelResultViewModel>();
                return;
            }

            model.SearchCriteriaMessage =
                "<p class='fs-5'>Showing results for hotels in: <strong>" +
                model.Destination +
                "</strong></p>";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetHotelsByCity";
            cmd.Parameters.AddWithValue("@City", model.Destination);

            try
            {
                DataSet ds = _db.GetDataSetUsingCmdObj(cmd);
                List<HotelResultViewModel> results = new List<HotelResultViewModel>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable table = ds.Tables[0];

                    int i;
                    for (i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow row = table.Rows[i];

                        HotelResultViewModel hotel = new HotelResultViewModel();
                        hotel.HotelID = Convert.ToInt32(row["HotelID"]);
                        hotel.HotelName = row["HotelName"].ToString();
                        hotel.City = row["City"].ToString();
                        hotel.Description = row["Description"].ToString();

                        if (row["Rating"] != DBNull.Value)
                            hotel.Rating = Convert.ToDecimal(row["Rating"]);

                        if (row["PricePerNight"] != DBNull.Value)
                            hotel.PricePerNight = Convert.ToDecimal(row["PricePerNight"]);

                        hotel.ImagePath = row["ImagePath"].ToString();

                        results.Add(hotel);
                    }
                }

                if (results.Count == 0)
                {
                    model.ErrorMessage = "No hotels were found matching your criteria.";
                }

                model.Results = results;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "A database error occurred. Error: " + ex.Message;
                model.Results = new List<HotelResultViewModel>();
            }
        }

        private void LoadHotelDetails(int hotelId, HotelDetailsViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetHotelsByID";
            cmd.Parameters.AddWithValue("@HotelID", hotelId);

            try
            {
                DataSet ds = _db.GetDataSetUsingCmdObj(cmd);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    model.HotelID = hotelId;
                    model.HotelName = row["HotelName"].ToString();
                    model.ImagePath = row["ImagePath"].ToString();
                    model.Address = row["Address"].ToString();
                    model.Phone = row["Phone"].ToString();
                    model.Email = row["Email"].ToString();
                    model.Description = row["Description"].ToString();

                    model.GalleryImages = new List<string>();

                    if (!string.IsNullOrEmpty(model.ImagePath))
                    {
                        model.GalleryImages.Add(model.ImagePath);

                        string extension = System.IO.Path.GetExtension(model.ImagePath);
                        string prefix = model.ImagePath.Replace(extension, "");

                        model.GalleryImages.Add(prefix + "_2" + extension);
                        model.GalleryImages.Add(prefix + "_3" + extension);
                    }
                    else
                    {
                        model.GalleryImages.Add("/images/hotels/default1.jpg");
                        model.GalleryImages.Add("/images/hotels/default2.jpg");
                        model.GalleryImages.Add("/images/hotels/default3.jpg");
                    }
                }
            }
            catch
            {
                model.HotelID = 0;
            }
        }

        // ======================== ROOM AVAILABILITY ========================
        private void LoadRoomAvailability(int hotelId, HotelDetailsViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAvailableRoomsByID";
            cmd.Parameters.AddWithValue("@HotelID", hotelId);

            try
            {
                DataSet ds = _db.GetDataSetUsingCmdObj(cmd);
                List<RoomViewModel> rooms = new List<RoomViewModel>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable table = ds.Tables[0];

                    int i;
                    for (i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow row = table.Rows[i];

                        RoomViewModel room = new RoomViewModel();
                        room.RoomID = Convert.ToInt32(row["RoomID"]);
                        room.RoomType = row["RoomType"].ToString();
                        room.ImagePath = row["ImagePath"].ToString();

                        if (row["Capacity"] != DBNull.Value)
                            room.Capacity = Convert.ToInt32(row["Capacity"]);

                        if (row["Price"] != DBNull.Value)
                            room.Price = Convert.ToDecimal(row["Price"]);

                        rooms.Add(room);
                    }
                }

                model.Rooms = rooms;
            }
            catch
            {
                model.Rooms = new List<RoomViewModel>();
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
