using System;

namespace TravelSiteModification.Models
{
    public class HotelBookingViewModel
    {
        // Booking Display Information
        public int HotelID { get; set; }
        public int RoomID { get; set; }

        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public string Capacity { get; set; }
        public decimal PricePerNight { get; set; }

        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }

        // Guest info fields (form submits these)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Optional payment (mock)
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }

        // Output messages
        public string StatusMessage { get; set; }
        public bool BookingSuccessful { get; set; }
    }
}
