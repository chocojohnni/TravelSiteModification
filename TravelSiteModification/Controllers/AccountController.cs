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
            LoginViewModel model = new LoginViewModel();

            // Check if we already have a saved login cookie
            string savedEmail = Request.Cookies["TravelSiteLoginEmail"];

            if (!string.IsNullOrEmpty(savedEmail))
            {
                model.Email = savedEmail;
                model.RememberMe = true;
                ViewBag.HasSavedLogin = true;
            }
            else
            {
                ViewBag.HasSavedLogin = false;
            }

            // If there is a TempData message (e.g., after clearing the cookie), pass it along
            if (TempData.ContainsKey("CookieMessage"))
            {
                ViewBag.CookieMessage = TempData["CookieMessage"].ToString();
            }

            return View(model);
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

                // Handle "remember me" cookie for faster logins
                if (model.RememberMe)
                {
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTimeOffset.UtcNow.AddDays(30);
                    options.IsEssential = true;
                    options.HttpOnly = true;
                    options.Secure = true;
                    options.SameSite = SameSiteMode.Lax;

                    Response.Cookies.Append("TravelSiteLoginEmail", model.Email, options);
                }
                else
                {
                    if (Request.Cookies.ContainsKey("TravelSiteLoginEmail"))
                    {
                        Response.Cookies.Delete("TravelSiteLoginEmail");
                    }
                }

                // Redirect after login
                if (HttpContext.Session.GetString("RedirectAfterLogin") != null)
                {
                    string redirectPage = HttpContext.Session.GetString("RedirectAfterLogin");
                    HttpContext.Session.Remove("RedirectAfterLogin");
                    return Redirect(redirectPage);
                }

                return RedirectToAction("Index", "TravelSite");
            }

            ModelState.AddModelError("", "Incorrect email or password.");
            return View(model);
        }

        // POST: /Account/ClearSavedLogin
        [HttpPost]
        public IActionResult ClearSavedLogin()
        {
            if (Request.Cookies.ContainsKey("TravelSiteLoginEmail"))
            {
                Response.Cookies.Delete("TravelSiteLoginEmail");
            }

            TempData["CookieMessage"] = "Saved login removed. You will need to enter your email next time.";

            return RedirectToAction("Login");
        }
    }
}
