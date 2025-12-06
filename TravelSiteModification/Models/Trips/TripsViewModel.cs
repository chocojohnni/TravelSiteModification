using System.Collections.Generic;

namespace TravelSiteModification.Models.Trips
{
    public class TripsViewModel
    {
        private bool isLoggedIn;

        private List<HotelBookingItem> hotelBookings;
        private List<FlightBookingItem> flightBookings;
        private List<CarBookingItem> carBookings;
        private List<EventBookingItem> eventBookings;
        private List<VacationPackageItem> vacationPackages;

        private bool hasHotelBookings;
        private bool hasFlightBookings;
        private bool hasCarBookings;
        private bool hasEventBookings;
        private bool hasPackages;

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { isLoggedIn = value; }
        }

        public List<HotelBookingItem> HotelBookings
        {
            get { return hotelBookings; }
            set { hotelBookings = value; }
        }

        public List<FlightBookingItem> FlightBookings
        {
            get { return flightBookings; }
            set { flightBookings = value; }
        }

        public List<CarBookingItem> CarBookings
        {
            get { return carBookings; }
            set { carBookings = value; }
        }

        public List<EventBookingItem> EventBookings
        {
            get { return eventBookings; }
            set { eventBookings = value; }
        }

        public List<VacationPackageItem> VacationPackages
        {
            get { return vacationPackages; }
            set { vacationPackages = value; }
        }

        public bool HasHotelBookings
        {
            get { return hasHotelBookings; }
            set { hasHotelBookings = value; }
        }

        public bool HasFlightBookings
        {
            get { return hasFlightBookings; }
            set { hasFlightBookings = value; }
        }

        public bool HasCarBookings
        {
            get { return hasCarBookings; }
            set { hasCarBookings = value; }
        }

        public bool HasEventBookings
        {
            get { return hasEventBookings; }
            set { hasEventBookings = value; }
        }

        public bool HasPackages
        {
            get { return hasPackages; }
            set { hasPackages = value; }
        }

        public TripsViewModel()
        {
            hotelBookings = new List<HotelBookingItem>();
            flightBookings = new List<FlightBookingItem>();
            carBookings = new List<CarBookingItem>();
            eventBookings = new List<EventBookingItem>();
            vacationPackages = new List<VacationPackageItem>();
        }
    }
}
