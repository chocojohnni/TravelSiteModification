using Microsoft.AspNetCore.Mvc;
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
        // "HotelDestination", "CheckInDate", "CheckOutDate", "SelectedHotelID", etc.
        public IActionResult Index()
        {
            var vm = new HotelIndexViewModel();

            // Read session values
            vm.Destination = HttpContext.Session.GetString("HotelDestination");
            vm.CheckInDate = HttpContext.Session.GetString("CheckInDate");
            vm.CheckOutDate = HttpContext.Session.GetString("CheckOutDate");

            if (string.IsNullOrEmpty(vm.Destination))
            {
                vm.Message = "No destination selected. Please return to the home page.";
                return View(vm);
            }

            vm.Message = $"Showing results for hotels in: {vm.Destination}";

            // Load hotels from DB
            try
            {
                DBConnect db = new DBConnect();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetHotelsByCityRefined";
                cmd.Parameters.AddWithValue("@City", vm.Destination);

                DataSet ds = db.GetDataSetUsingCmdObj(cmd);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var hotel = new HotelViewModel
                        {
                            Id = Convert.ToInt32(row["HotelID"]),
                            Name = row["HotelName"].ToString(),
                            ImagePath = row["ImagePath"].ToString(),
                            Address = row["Address"].ToString(),
                            Phone = row["Phone"].ToString(),
                            Email = row["Email"].ToString(),
                            Description = row["Description"].ToString()
                        };
                        vm.Hotels.Add(hotel);
                    }
                }
                else
                {
                    vm.Message += " — No hotels found in this city.";
                }
            }
            catch (Exception ex)
            {
                vm.Message = $"A database error occurred. Error: {ex.Message}";
            }

            return View(vm);
        }

        // POST: Hotel/ChangeDestination
        [HttpPost]
        public IActionResult ChangeDestination(string newDestination, string checkInDate, string checkOutDate)
        {
            if (string.IsNullOrEmpty(newDestination))
            {
                TempData["DestinationError"] = "Please select a valid destination.";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("HotelDestination", newDestination);
            HttpContext.Session.SetString("CheckInDate", checkInDate);
            HttpContext.Session.SetString("CheckOutDate", checkOutDate);

            return RedirectToAction("Index");
        }

        // GET: Hotel/Details/5
        public IActionResult Details(int id)
        {
            var hotel = new HotelViewModel();
            var rooms = new List<RoomViewModel>();

            try
            {
                DBConnect db = new DBConnect();

                // Hotel details
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetHotelsByIDRefined";
                cmd.Parameters.AddWithValue("@HotelID", id);

                DataSet ds = db.GetDataSetUsingCmdObj(cmd);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    hotel.Id = Convert.ToInt32(row["HotelID"]);
                    hotel.Name = row["HotelName"].ToString();
                    hotel.ImagePath = row["ImagePath"].ToString();
                    hotel.Address = row["Address"].ToString();
                    hotel.Phone = row["Phone"].ToString();
                    hotel.Email = row["Email"].ToString();
                    hotel.Description = row["Description"].ToString();
                }
                else
                {
                    TempData["Error"] = "Hotel not found.";
                    return RedirectToAction("Index");
                }

                // Room availability
                SqlCommand cmdRooms = new SqlCommand();
                cmdRooms.CommandType = CommandType.StoredProcedure;
                cmdRooms.CommandText = "GetAvailableRoomsByIDRefined";
                cmdRooms.Parameters.AddWithValue("@HotelID", id);

                DataSet rds = db.GetDataSetUsingCmdObj(cmdRooms);
                if (rds != null && rds.Tables.Count > 0 && rds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in rds.Tables[0].Rows)
                    {
                        rooms.Add(new RoomViewModel
                        {
                            Id = Convert.ToInt32(r["RoomID"]),
                            RoomName = Convert.ToString(r["RoomName"]),
                            Description = Convert.ToString(r["Description"]),
                            Price = Convert.ToDecimal(r["Price"]),
                            MaxOccupancy = Convert.ToInt32(r["MaxOccupancy"])
                        });

                    }
                }

                // store selected hotel
                HttpContext.Session.SetInt32("SelectedHotelID", id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading hotel details: {ex.Message}";
                return RedirectToAction("Index");
            }

            // prepare view bag with checkin/checkout
            ViewBag.CheckInDate = HttpContext.Session.GetString("CheckInDate");
            ViewBag.CheckOutDate = HttpContext.Session.GetString("CheckOutDate");

            ViewBag.Rooms = rooms;
            return View(hotel);
        }

        // POST: Hotel/SelectRoom
        [HttpPost]
        public IActionResult SelectRoom(int roomId, string checkInDate, string checkOutDate)
        {
            // store selected room and dates and redirect to booking page
            HttpContext.Session.SetInt32("SelectedRoomID", roomId);
            HttpContext.Session.SetString("CheckInDate", checkInDate);
            HttpContext.Session.SetString("CheckOutDate", checkOutDate);

            // Redirect to HotelBooking controller
            return RedirectToAction("Index", "HotelBooking");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "TravelSite");
        }
    }
}
