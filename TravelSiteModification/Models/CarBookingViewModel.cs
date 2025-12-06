namespace TravelSiteModification.Models
{
    public class CarBookingViewModel
    {
        private int selectedCarID;
        private string carModel;
        private string carType;
        private string agencyName;
        private string pickupLocation;
        private string dropoffLocation;

        private string pickupDate;
        private string dropoffDate;

        private int totalDays;
        private decimal totalCost;

        private string firstName;
        private string lastName;
        private string email;

        private string cardNumber;
        private string expiryDate;
        private string cvv;

        private string status;
        private bool bookingConfirmed;

        public int SelectedCarID
        {
            get { return selectedCarID; }
            set { selectedCarID = value; }
        }

        public string CarModel
        {
            get { return carModel; }
            set { carModel = value; }
        }

        public string CarType
        {
            get { return carType; }
            set { carType = value; }
        }

        public string AgencyName
        {
            get { return agencyName; }
            set { agencyName = value; }
        }

        public string PickupLocation
        {
            get { return pickupLocation; }
            set { pickupLocation = value; }
        }

        public string DropoffLocation
        {
            get { return dropoffLocation; }
            set { dropoffLocation = value; }
        }

        public string PickupDate
        {
            get { return pickupDate; }
            set { pickupDate = value; }
        }

        public string DropoffDate
        {
            get { return dropoffDate; }
            set { dropoffDate = value; }
        }

        public int TotalDays
        {
            get { return totalDays; }
            set { totalDays = value; }
        }

        public decimal TotalCost
        {
            get { return totalCost; }
            set { totalCost = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        public string ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }

        public string CVV
        {
            get { return cvv; }
            set { cvv = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public bool BookingConfirmed
        {
            get { return bookingConfirmed; }
            set { bookingConfirmed = value; }
        }
    }
}