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
        public IActionResult Index()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CheckUserEmail";
            cmd.Parameters.AddWithValue("@Email", model.Email);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables[0].Rows.Count > 0)
            {
                model.Message = "An account with this email is already registered.";
                return View(model);
            }

            // Insert new user
            SqlCommand add = new SqlCommand();
            add.CommandType = CommandType.StoredProcedure;
            add.CommandText = "AddUser";

            add.Parameters.AddWithValue("@FName", model.FirstName);
            add.Parameters.AddWithValue("@LName", model.LastName);
            add.Parameters.AddWithValue("@Email", model.Email);
            add.Parameters.AddWithValue("@Password", model.Password);
            add.Parameters.AddWithValue("@IsActive", true);
            add.Parameters.AddWithValue("@DateCreated", DateTime.Now);

            DataSet userDs = db.GetDataSetUsingCmdObj(add);

            if (userDs.Tables.Count == 0 || userDs.Tables[0].Rows.Count == 0)
            {
                model.Message = "User cannot be created";
                return View(model);
            }

            int userID = Convert.ToInt32(userDs.Tables[0].Rows[0]["UserID"]);

            HttpContext.Session.SetString("UserFirstName", model.FirstName);
            HttpContext.Session.SetString("UserEmail", model.Email);
            HttpContext.Session.SetInt32("UserID", userID);

            return RedirectToAction("Index", "TravelSite");
        }
    }
}
