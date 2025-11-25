namespace TravelSiteModification.Models
{
    public class HotelIndexViewModel
    {
        private string message;
        private string destination;
        private string checkInDate;
        private string checkOutDate;
        private List<HotelViewModel> hotels;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public string CheckInDate
        {
            get { return checkInDate; }
            set { checkInDate = value; }
        }

        public string CheckOutDate
        {
            get { return checkOutDate; }
            set { checkOutDate = value; }
        }

        public List<HotelViewModel> Hotels
        {
            get { return hotels; }
            set { hotels = value; }
        }

        public HotelIndexViewModel()
        {
            hotels = new List<HotelViewModel>();
        }
    }
}
