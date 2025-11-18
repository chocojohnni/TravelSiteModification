using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class FlightBookingViewModel
    {
        private int flightId;
        private string airlineName;
        private string departureCity;
        private string arrivalCity;
        private DateTime departureTime;
        private DateTime arrivalTime;
        private decimal price;

        private string firstName;
        private string lastName;
        private string email;
        private string cardNumber;
        private string expiryDate;
        private string cvv;

        private string statusMessage;
        private bool isSuccess;

        // Flight info
        public int FlightId
        {
            get { return flightId; }
            set { flightId = value; }
        }

        public string AirlineName
        {
            get { return airlineName; }
            set { airlineName = value; }
        }

        public string DepartureCity
        {
            get { return departureCity; }
            set { departureCity = value; }
        }

        public string ArrivalCity
        {
            get { return arrivalCity; }
            set { arrivalCity = value; }
        }

        public DateTime DepartureTime
        {
            get { return departureTime; }
            set { departureTime = value; }
        }

        public DateTime ArrivalTime
        {
            get { return arrivalTime; }
            set { arrivalTime = value; }
        }

        [Display(Name = "Price")]
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        // Passenger info
        [Required]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        [Required]
        [EmailAddress]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        // Payment 
        [Required]
        [Display(Name = "Card Number")]
        [RegularExpression(@"^\d{13,16}$", ErrorMessage = "Enter a valid card number.")]
        public string CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        [Required]
        [Display(Name = "Expiry (MM/YY)")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Use MM/YY format.")]
        public string ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }

        [Required]
        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Enter a valid CVV.")]
        public string CVV
        {
            get { return cvv; }
            set { cvv = value; }
        }

        public string StatusMessage
        {
            get { return statusMessage; }
            set { statusMessage = value; }
        }

        public bool IsSuccess
        {
            get { return isSuccess; }
            set { isSuccess = value; }
        }
    }
}

