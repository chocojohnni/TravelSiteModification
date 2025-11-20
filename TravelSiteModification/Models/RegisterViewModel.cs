using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class RegisterViewModel
    {
        private string firstName;
        private string lastName;
        private string email;
        private string password;
        private string message;

        [Required]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [Required]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        [Required, EmailAddress]
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

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
