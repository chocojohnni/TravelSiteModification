using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        private string Email { get; set; }

        [Required]
        private string Password { get; set; }

        private string Message { get; set; }
    }
}
