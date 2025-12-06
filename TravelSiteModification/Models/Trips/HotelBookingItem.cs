namespace TravelSiteModification.Models.Trips
{
    public class HotelBookingItem
    {
        private string hotelName;
        private string roomType;
        private string status;
        private string totalPrice;

        private string checkInDate;
        private string checkOutDate;

        public string HotelName
        {
            get { return hotelName; }
            set { hotelName = value; }
        }

        public string RoomType
        {
            get { return roomType; }
            set { roomType = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
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
    }
}
