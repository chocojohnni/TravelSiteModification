using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;
using TravelSiteModification.Models;
using Utilities;

namespace TravelSiteModification.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Edit()
        {
            return View(new EditProfileViewModel());
        }

        [HttpPost]
        public IActionResult Edit(EditProfileViewModel model)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserID");
            int userID;

            if (sessionUserId == null)
            {
                model.Message = "You must be logged in to edit your profile.";
                return View(model);
            }
            else
            {
                userID = (int)sessionUserId;
            }

            DBConnect objDB = new DBConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateUserProfile";

            cmd.Parameters.AddWithValue("@UserID", userID);

            string emailToStore;
            if (model.Email == null)
            {
                emailToStore = "";
            }
            else
            {
                emailToStore = model.Email;
            }
            cmd.Parameters.AddWithValue("@NewEmail", emailToStore);

            string passwordToStore;
            if (model.Password == null)
            {
                passwordToStore = "";
            }
            else
            {
                passwordToStore = model.Password;
            }
            cmd.Parameters.AddWithValue("@NewPW", passwordToStore);

            cmd.Parameters.AddWithValue("@Address", model.Address);
            cmd.Parameters.AddWithValue("@City", model.City);
            cmd.Parameters.AddWithValue("@State", model.State);
            cmd.Parameters.AddWithValue("@Zip", model.Zip);
            cmd.Parameters.AddWithValue("@Phone", model.Phone);

            cmd.Parameters.AddWithValue("@CardNumber", model.CardNumber);
            cmd.Parameters.AddWithValue("@CardType", model.CardType);
            cmd.Parameters.AddWithValue("@ExpDate", model.Expiry);
            cmd.Parameters.AddWithValue("@CVV", model.CVV);
            cmd.Parameters.AddWithValue("@CardHolderName", model.CardHolder);

            objDB.GetDataSetUsingCmdObj(cmd);

            model.Message = "Profile updated successfully!";

            return View(model);
        }
    }
}
