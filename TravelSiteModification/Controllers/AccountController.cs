using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBConnect db;

        // Create a new database connection
        public AccountController()
        {
            db = new DBConnect();
        }
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CheckCredentials";
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Password", model.Password);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables[0].Rows.Count == 1)
            {
                var row = ds.Tables[0].Rows[0];
                string firstName = row["FirstName"].ToString();
                int userID = (int)row["UserID"];

                // Save session values
                HttpContext.Session.SetString("UserFirstName", firstName);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetInt32("UserID", userID);

                // Redirect after login
                if (HttpContext.Session.GetString("RedirectAfterLogin") != null)
                {
                    string redirectPage = HttpContext.Session.GetString("RedirectAfterLogin");
                    HttpContext.Session.Remove("RedirectAfterLogin");
                    return Redirect(redirectPage);
                }

                return RedirectToAction("Index", "TravelSiteModification");
            }

            ModelState.AddModelError("", "Incorrect email or password.");
            return View(model);
        }
    }
}
