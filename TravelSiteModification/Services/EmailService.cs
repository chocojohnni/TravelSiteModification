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

            String subject = "Request to Reset Your TravelSite Password";

            String body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8' />
                <title>Password Reset</title>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 24px; border-radius: 4px;'>
                    <h2 style='color: #00695c; margin-top: 0;'>Hello,</h2>

                    <p>We received a request to reset the password for your TravelSite account.</p>
                    <p>If you made this request, click the button below to verify your identity and continue resetting your password.</p>

                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}'
                           style='display: inline-block;
                                  padding: 12px 24px;
                                  background-color: #00695c;
                                  color: #ffffff;
                                  text-decoration: none;
                                  border-radius: 4px;
                                  font-weight: 600;'>
                            Verify &amp; Reset Password
                        </a>
                    </p>

                    <p>If the button above does not work, you can copy and paste this link into your browser:</p>
                    <p>
                        <a href='{resetLink}' style='color: #00695c; word-break: break-all;'>
                            {resetLink}
                        </a>
                    </p>

                    <p>If you did not request a password reset, you can safely ignore this email and your password will remain unchanged.</p>

                    <p style='margin-top: 30px;'>
                        Thanks,<br />
                        TravelSite
                    </p>
                </div>
            </body>
            </html>";

            try
            {
                emailService.SendMail(
                    recipient: toEmail,
                    sender: "tup84792@temple.edu",
                    subject: subject,
                    body: body
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send password reset email: {ex.Message}", ex);
            }
        }
    }
}
