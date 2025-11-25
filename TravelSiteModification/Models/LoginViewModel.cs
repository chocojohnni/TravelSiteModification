using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class LoginViewModel
    {
        private string email;
        private string password;
        private bool rememberMe;

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool RememberMe
        {
            get { return rememberMe; }
            set { rememberMe = value; }
        }
    }
}
