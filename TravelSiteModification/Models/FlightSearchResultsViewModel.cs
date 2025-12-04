namespace TravelSiteModification.Models
{
    public class FlightSearchResultsViewModel
    {
        public FlightSearchRequest Search { get; set; }
        public List<FlightDto> Flights { get; set; } = new List<FlightDto>();
    }
}
