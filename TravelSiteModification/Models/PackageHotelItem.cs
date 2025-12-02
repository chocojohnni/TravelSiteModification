namespace TravelSiteModification.Models
{
    public class PackageHotelItem
    {
        private int packageHotelId;
        private int hotelId;
        private int roomId;
        private string hotelName;
        private string city;
        private string state;
        private DateTime checkInDate;
        private DateTime checkOutDate;
        private decimal totalPrice;

        public int PackageHotelId
        {
            get { return packageHotelId; }
            set { packageHotelId = value; }
        }

        public int HotelId
        {
            get { return hotelId; }
            set { hotelId = value; }
        }

        public int RoomId
        {
            get { return roomId; }
            set { roomId = value; }
        }

        public string HotelName
        {
            get { return hotelName; }
            set { hotelName = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public DateTime CheckInDate
        {
            get { return checkInDate; }
            set { checkInDate = value; }
        }

        public DateTime CheckOutDate
        {
            get { return checkOutDate; }
            set { checkOutDate = value; }
        }

        public decimal TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
    }
}
