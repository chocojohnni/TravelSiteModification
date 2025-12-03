namespace TravelSiteModification.Models
{
    public class FlightSearchViewModel
    {
        public string DepCity { get; set; }
        public string DepState { get; set; }
        public string ArrCity { get; set; }
        public string ArrState { get; set; }

        // Optional requirements
        public int? AirlineID { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? NonStop { get; set; }
        public bool? FirstClass { get; set; }

        // Data to display
        public List<Airline> Carriers { get; set; } = new List<Airline>();
        public List<Flight> Flights { get; set; } = new List<Flight>();

        public string ErrorMessage { get; set; }
    }
}
