using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class LoginViewModel
    {
        private string email;
        private string password;
        private bool rememberMe;

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
