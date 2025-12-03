namespace TravelSiteModification.Models.Trips
{
    public class FlightBookingItem
    {
        public string AirlineName { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }

        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }

        public string Status { get; set; }
        public string TotalPrice { get; set; }
    }
}
