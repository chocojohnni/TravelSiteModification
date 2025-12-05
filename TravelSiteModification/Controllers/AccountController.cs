using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using TravelSiteModification.Services;
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

                // ⭐ NEW: This is what your homepage expects
                ViewBag.FirstName = firstName;
                ViewBag.UserFirstName = firstName; // optional but safe for other pages

                // Remember me cookie
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


        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session values
            HttpContext.Session.Clear();

            // OPTIONAL: Remove login cookie if it exists
            if (Request.Cookies.ContainsKey("TravelSiteLoginEmail"))
            {
                Response.Cookies.Delete("TravelSiteLoginEmail");
            }

            // Redirect to homepage
            return RedirectToAction("Index", "Home");
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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Please enter your email.";
                return View();
            }

            // check if the email exists
            SqlCommand checkCmd = new SqlCommand();
            checkCmd.CommandType = CommandType.StoredProcedure;
            checkCmd.CommandText = "CheckUserEmail";
            checkCmd.Parameters.AddWithValue("@Email", email);

            DataSet ds = db.GetDataSetUsingCmdObj(checkCmd);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                ViewBag.Error = "No account found with that email.";
                return View();
            }

            // 2) Load user's security questions 
            SqlCommand qsCmd = new SqlCommand();
            qsCmd.CommandType = CommandType.StoredProcedure;
            qsCmd.CommandText = "TP_GetUserSecurityQuestions";
            qsCmd.Parameters.AddWithValue("@Email", email);

            DataSet qs = db.GetDataSetUsingCmdObj(qsCmd);

            if (qs.Tables.Count == 0 || qs.Tables[0].Rows.Count == 0)
            {
                ViewBag.Error = "No security questions are set up for this account.";
                return View();
            }

            // Pick a random question
            Random rand = new Random();
            int index = rand.Next(0, qs.Tables[0].Rows.Count);

            DataRow row = qs.Tables[0].Rows[index];

            string questionText = row["QuestionText"].ToString();
            string correctAnswer = row["Answer"].ToString();
            int userID = Convert.ToInt32(row["UserID"]);

            HttpContext.Session.SetInt32("Reset_UserID", userID);
            HttpContext.Session.SetString("Reset_Question", questionText);
            HttpContext.Session.SetString("Reset_Answer", correctAnswer);

            string firstName = string.Empty;

            SqlCommand userCmd = new SqlCommand();
            userCmd.CommandType = CommandType.Text;
            userCmd.CommandText = "SELECT FirstName FROM Users WHERE Email = @Email";
            userCmd.Parameters.AddWithValue("@Email", email);

            DataSet userDs = db.GetDataSetUsingCmdObj(userCmd);

            if (userDs.Tables.Count > 0 && userDs.Tables[0].Rows.Count > 0)
            {
                firstName = Convert.ToString(userDs.Tables[0].Rows[0]["FirstName"]);
            }

            string resetLink = Url.Action(
                "VerifySecurityQuestion",
                "Account",
                new { email = email },
                Request.Scheme
            );

            try
            {
                EmailService.SendPasswordResetEmail(email, firstName, resetLink);

                ViewBag.Message = "We've emailed you a link to continue resetting your password. Please check your inbox.";
            }
            catch (Exception)
            {
                ViewBag.Error = "We were unable to send the reset email at this time. Please try again later.";
            }
            return View();
        }

        [HttpGet]
        public IActionResult VerifySecurityQuestion()
        {
            if (HttpContext.Session.GetString("Reset_Question") == null)
            {
                return RedirectToAction("ForgotPassword");
            }
            ViewBag.Question = HttpContext.Session.GetString("Reset_Question");
            return View();
        }

        [HttpPost]
        public IActionResult VerifySecurityQuestion(string answer)
        {
            string correctAnswer = HttpContext.Session.GetString("Reset_Answer");

            if (correctAnswer == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            if (!string.Equals(answer.Trim(), correctAnswer.Trim()))
            {
                ViewBag.Error = "Incorrect answer. Please try again.";
                ViewBag.Question = HttpContext.Session.GetString("Reset_Question");
                return View();
            }

            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (HttpContext.Session.GetInt32("Reset_UserID") == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            int userID = HttpContext.Session.GetInt32("Reset_UserID") ?? -1;

            if (userID == -1)
            {
                return RedirectToAction("ForgotPassword");
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_UpdateUserPassword";
            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@NewPassword", newPassword);

            db.DoUpdateUsingCmdObj(cmd);

            // Clear session afterwards
            HttpContext.Session.Remove("Reset_UserID");
            HttpContext.Session.Remove("Reset_Question");
            HttpContext.Session.Remove("Reset_Answer");

            TempData["Success"] = "Your password has been reset successfully!";
            return RedirectToAction("Login");
        }
    }
}
