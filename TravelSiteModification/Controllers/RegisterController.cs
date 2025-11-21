using Microsoft.AspNetCore.Mvc;
using TravelSiteModification.Models;
using System.Data;
using System.Data.SqlClient;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class RegisterController : Controller
    {
        private readonly DBConnect db;

        // Create new database connection
        public RegisterController()
        {
            db = new DBConnect();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email exists
            SqlCommand check = new SqlCommand();
            check.CommandType = CommandType.StoredProcedure;
            check.CommandText = "CheckUserEmail";
            check.Parameters.AddWithValue("@Email", model.Email);

            DataSet ds = db.GetDataSetUsingCmdObj(check);

            if (ds.Tables[0].Rows.Count > 0)
            {
                model.Message = "An account with this email already exists.";
                return View(model);
            }

            // Insert User
            SqlCommand add = new SqlCommand();
            add.CommandType = CommandType.StoredProcedure;
            add.CommandText = "AddUser";

            add.Parameters.AddWithValue("@FName", model.FirstName);
            add.Parameters.AddWithValue("@LName", model.LastName);
            add.Parameters.AddWithValue("@Email", model.Email);
            add.Parameters.AddWithValue("@Password", model.Password);
            add.Parameters.AddWithValue("@IsActive", true);
            add.Parameters.AddWithValue("@DateCreated", DateTime.Now);

            // Run the INSERT
            DataSet dsUser = db.GetDataSetUsingCmdObj(add);

            int userID = 0;

            if (dsUser.Tables.Count > 0 && dsUser.Tables[0].Rows.Count > 0)
            {
                userID = Convert.ToInt32(dsUser.Tables[0].Rows[0]["UserID"]);
            }
            else
            {
                model.Message = "Account creation failed. Please refresh the page and try again.";
                return View(model);
            }

                // Save session
                HttpContext.Session.SetString("UserFirstName", model.FirstName);
            HttpContext.Session.SetString("UserEmail", model.Email);
            HttpContext.Session.SetInt32("UserID", userID);

            return RedirectToAction("Index", "TravelSite");
        }
    }
}
