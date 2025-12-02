namespace TravelSiteModification.Models
{
    public class PackageFlightItem
    {
        private int packageFlightId;
        private int flightId;
        private string airlineName;
        private string departureCity;
        private string arrivalCity;
        private DateTime departureTime;
        private DateTime arrivalTime;
        private decimal price;

        public int PackageFlightId
        {
            get { return packageFlightId; }
            set { packageFlightId = value; }
        }

        public int FlightId
        {
            get { return flightId; }
            set { flightId = value; }
        }

        public string AirlineName
        {
            get { return airlineName; }
            set { airlineName = value; }
        }

        public string DepartureCity
        {
            get { return departureCity; }
            set { departureCity = value; }
        }

        public string ArrivalCity
        {
            get { return arrivalCity; }
            set { arrivalCity = value; }
        }

        public DateTime DepartureTime
        {
            get { return departureTime; }
            set { departureTime = value; }
        }

        public DateTime ArrivalTime
        {
            get { return arrivalTime; }
            set { arrivalTime = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
