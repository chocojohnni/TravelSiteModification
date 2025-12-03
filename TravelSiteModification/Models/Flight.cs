namespace TravelSiteModification.Models
{
    public class Flight
    {
        private int flightID;
        private int airlineID;
        private string departureCity;
        private string departureState;
        private string arrivalCity;
        private string arrivalState;
        private DateTime departureTime;
        private DateTime arrivalTime;
        private decimal price;
        private int seatsAvailable;
        private bool nonStop;
        private bool firstClass;
        private string airlineImage; // UI-only

        public int FlightID
        {
            get { return flightID; } 
            set { flightID = value; }
        }

        public int AirlineID
        {
            get { return airlineID; }
            set { airlineID = value; }
        }

        public string DepartureCity
        {
            get { return departureCity; }
            set { departureCity = value; }
        }

        public string DepartureState
        {
            get { return departureState; }
            set { departureState = value; }
        }

        public string ArrivalCity
        {
            get { return arrivalCity; }
            set { arrivalCity = value; }
        }

        public string ArrivalState
        {
            get { return arrivalState; }
            set { arrivalState = value; }
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

        public int SeatsAvailable
        {
            get { return seatsAvailable; }
            set { seatsAvailable = value; }
        }

        public bool NonStop
        {
            get { return nonStop; }
            set { nonStop = value; }
        }

        public bool FirstClass
        {
            get { return firstClass; }
            set { firstClass = value; }
        }

        public string AirlineImage
        {
            get { return airlineImage; }
            set { airlineImage = value; }
        }
    }
}
