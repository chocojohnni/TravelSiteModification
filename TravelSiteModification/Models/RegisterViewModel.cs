using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        //Security Question stuff
        private int question1Id;
        private string answer1;
        private int question2Id;
        private string answer2;
        private int question3Id;
        private string answer3;
        private List<SelectListItem> securityQuestions;

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

        [Required(ErrorMessage = "Please select your first security question.")]
        public int Question1Id
        {
            get { return question1Id; }
            set { question1Id = value; }
        }

        [Required(ErrorMessage = "Please provide an answer.")]
        [MaxLength(255)]
        public string Answer1
        {
            get { return answer1; }
            set { answer1 = value; }
        }

        [Required(ErrorMessage = "Please select your second security question.")]
        public int Question2Id
        {
            get { return question2Id; }
            set { question2Id = value; }
        }

        [Required(ErrorMessage = "Please provide an answer.")]
        [MaxLength(255)]
        public string Answer2
        {
            get { return answer2; }
            set { answer2 = value; }
        }

        [Required(ErrorMessage = "Please select your third security question.")]
        public int Question3Id
        {
            get { return question3Id; }
            set { question3Id = value; }
        }

        [Required(ErrorMessage = "Please provide an answer.")]
        [MaxLength(255)]
        public string Answer3
        {
            get { return answer3; }
            set { answer3 = value; }
        }
        public List<SelectListItem>? SecurityQuestions
        {
            get { return securityQuestions; }
            set { securityQuestions = value; }
        }
    }
}
