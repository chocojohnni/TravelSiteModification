namespace TravelSiteModification.Models
{
    public class CarBookingViewModel
    {
        // Selected car details
        public int SelectedCarID { get; set; }

        public string CarModel { get; set; }
        public string CarType { get; set; }
        public string AgencyName { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }

        public string PickupDate { get; set; }
        public string DropoffDate { get; set; }

        public int TotalDays { get; set; }
        public decimal TotalCost { get; set; }

        // Driver info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Payment (mock)
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }

        // Status
        public string Status { get; set; }
        public bool BookingConfirmed { get; set; }
    }
}
