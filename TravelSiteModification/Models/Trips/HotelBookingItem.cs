namespace TravelSiteModification.Models.Trips
{
    public class HotelBookingItem
    {
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public string Status { get; set; }
        public string TotalPrice { get; set; }

        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
    }
}
