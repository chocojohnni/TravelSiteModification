namespace TravelSiteModification.Models
{
    public class HotelBookingViewModel
    {
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhone { get; set; }
        public string HotelEmail { get; set; }

        public string RoomType { get; set; }
        public decimal PricePerNight { get; set; }

        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }

        public int TotalNights { get; set; }
        public decimal TotalPrice { get; set; }

        // Customer info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
