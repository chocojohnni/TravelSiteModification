using Microsoft.AspNetCore.Mvc;
using TravelSiteModification.Models;
using System.Data;
using System.Data.SqlClient;
using Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TravelSiteModification.Controllers
{
    public class RegisterController : Controller
    {
        private readonly DBConnect db;
        public RegisterController()
        {
            db = new DBConnect();
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            LoadSecurityQuestions(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadSecurityQuestions(model);
                return View(model);
            }

            // Check if email already exists
            SqlCommand check = new SqlCommand();
            check.CommandType = CommandType.StoredProcedure;
            check.CommandText = "CheckUserEmail";
            check.Parameters.AddWithValue("@Email", model.Email);

            DataSet ds = db.GetDataSetUsingCmdObj(check);

            if (ds.Tables[0].Rows.Count > 0)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                LoadSecurityQuestions(model);
                return View(model);
            }

            SqlCommand add = new SqlCommand();
            add.CommandType = CommandType.StoredProcedure;
            add.CommandText = "AddUser";

            add.Parameters.AddWithValue("@FName", model.FirstName);
            add.Parameters.AddWithValue("@LName", model.LastName);
            add.Parameters.AddWithValue("@Email", model.Email);
            add.Parameters.AddWithValue("@Password", model.Password);
            add.Parameters.AddWithValue("@IsActive", true);
            add.Parameters.AddWithValue("@DateCreated", DateTime.Now);

            DataSet dsUser = db.GetDataSetUsingCmdObj(add);

            int userID = 0;

            if (dsUser.Tables.Count > 0 && dsUser.Tables[0].Rows.Count > 0)
            {
                userID = Convert.ToInt32(dsUser.Tables[0].Rows[0]["UserID"]);
            }
            else
            {
                ModelState.AddModelError("", "There was a problem creating your account. Please try again.");
                LoadSecurityQuestions(model);
                return View(model);
            }

            // Insert the user's security questions and answers
            SqlCommand addQuestions = new SqlCommand();
            addQuestions.CommandType = CommandType.StoredProcedure;
            addQuestions.CommandText = "TP_AddUserSecurityQuestions";

            addQuestions.Parameters.AddWithValue("@UserID", userID);
            addQuestions.Parameters.AddWithValue("@Question1ID", model.Question1Id);
            addQuestions.Parameters.AddWithValue("@Answer1", model.Answer1);
            addQuestions.Parameters.AddWithValue("@Question2ID", model.Question2Id);
            addQuestions.Parameters.AddWithValue("@Answer2", model.Answer2);
            addQuestions.Parameters.AddWithValue("@Question3ID", model.Question3Id);
            addQuestions.Parameters.AddWithValue("@Answer3", model.Answer3);

            int rowsAffected = db.DoUpdateUsingCmdObj(addQuestions);

            if (rowsAffected < 3)
            {
                ModelState.AddModelError("", "There was a problem saving your security questions. Please try again.");

                // Reload
                LoadSecurityQuestions(model);
                return View(model);
            }

            HttpContext.Session.SetString("UserFirstName", model.FirstName);
            HttpContext.Session.SetString("UserEmail", model.Email);
            HttpContext.Session.SetInt32("UserID", userID);

            return RedirectToAction("Index", "TravelSite");
        }

        private void LoadSecurityQuestions(RegisterViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetSecurityQuestions";

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            model.SecurityQuestions = new List<SelectListItem>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = row["QuestionID"].ToString();
                    item.Text = row["QuestionText"].ToString();
                    model.SecurityQuestions.Add(item);
                }
            }
        }
    }
}
