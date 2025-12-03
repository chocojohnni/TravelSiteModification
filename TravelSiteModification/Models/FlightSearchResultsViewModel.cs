namespace TravelSiteModification.Models
{
    public class FlightSearchResultsViewModel
    {
        private FlightSearchViewModel search;
        private List<Flight> flights;

        public FlightSearchViewModel Search
        {
            get { return search; }
            set { search = value; }
        }

        public List<Flight> Flights
        {
            get { return flights; }
            set { flights = value; }
        }
    }
}
