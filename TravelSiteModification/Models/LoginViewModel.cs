using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class LoginViewModel
    {
        private string email;
        private string password;

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, include one uppercase letter and one number.")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
