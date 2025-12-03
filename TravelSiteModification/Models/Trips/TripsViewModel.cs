using System.Collections.Generic;

namespace TravelSiteModification.Models.Trips
{
    public class TripsViewModel
    {
        public bool IsLoggedIn { get; set; }

        public List<HotelBookingItem> HotelBookings { get; set; }
        public List<FlightBookingItem> FlightBookings { get; set; }
        public List<CarBookingItem> CarBookings { get; set; }
        public List<EventBookingItem> EventBookings { get; set; }
        public List<VacationPackageItem> VacationPackages { get; set; }

        public bool HasHotelBookings { get; set; }
        public bool HasFlightBookings { get; set; }
        public bool HasCarBookings { get; set; }
        public bool HasEventBookings { get; set; }
        public bool HasPackages { get; set; }

        public TripsViewModel()
        {
            HotelBookings = new List<HotelBookingItem>();
            FlightBookings = new List<FlightBookingItem>();
            CarBookings = new List<CarBookingItem>();
            EventBookings = new List<EventBookingItem>();
            VacationPackages = new List<VacationPackageItem>();
        }
    }
}
