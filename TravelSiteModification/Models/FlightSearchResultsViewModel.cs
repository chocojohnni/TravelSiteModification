namespace TravelSiteModification.Models
{
    public class FlightSearchResultsViewModel
    {
        private FlightSearchRequest search;
        private List<FlightDto> flights;

        public FlightSearchRequest Search
        {
            get { return search; }
            set { search = value; }
        }

        public List<FlightDto> Flights
        {
            get { return flights; }
            set { flights = value; }
        }
    }
}
