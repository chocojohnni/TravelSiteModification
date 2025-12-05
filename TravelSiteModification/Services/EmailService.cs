using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace TravelSiteModification.Services
{
    public class EmailService
    {
        public static void SendPasswordResetEmail(String toEmail, String firstName, String resetLink)
        {
            Email emailService = new Email();
            emailService.HTMLBody = true;

            String subject = "Request to Reset TravelSite Password";
            String body = "";

            try
            {
                emailService.SendMail(
                    recipient: toEmail,
                    sender: "tun31378@temple.edu",
                    subject: subject,
                    body: body
                    );
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send password reset email: {ex.Message}", ex);

            }
        }
        public static void SendEmailConfirmation(User user, DataSet packageDetails, decimal totalCost)
        {
            Email emailService = new Email();
            String subject = "Booking Confirmation - VacationBooking";

            if (user == null || String.IsNullOrEmpty(user.Email))
            {
                return;
            }

            StringBuilder itemsList = new StringBuilder();

            foreach (DataRow row in packageDetails.Tables[0].Rows)
            {

                itemsList.Append($@"
                    <tr>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{row["Type"]}</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{row["ItemName"]}</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{row["Details"]}</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: right;'>${row["TotalCost"]}</td>
                    </tr>
                ");
            }

            string body = $@"";

            try
            {
                emailService.SendMail(
                    recipient: user.Email,
                    sender: "tup84792@temple.edu",
                    subject: subject,
                    body: body
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send confirmation email: {ex.Message}", ex);
            }


        }
    }
}
