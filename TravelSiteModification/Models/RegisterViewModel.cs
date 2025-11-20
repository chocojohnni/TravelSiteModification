using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class RegisterViewModel
    {
        private string firstName;
        private string lastName;
        private string email;
        private string password;
        private bool isActive;
        private DateTime dateCreated;
        private string message;

        [Required]
        [RegularExpression(@"^[A-Za-z'-]{2,30}$",
            ErrorMessage = "First name must be 2–30 letters and contain only letters, hyphens, or apostrophes.")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [Required]
        [RegularExpression(@"^[A-Za-z'-]{2,30}$",
            ErrorMessage = "Last name must be 2–30 letters and contain only letters, hyphens, or apostrophes.")]

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{6,}$",
            ErrorMessage = "Password must contain upper, lower, number, and at least 6 characters.")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
