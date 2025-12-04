namespace TravelSiteModification.Models
{
    public class FlightReserveRequest
    {
        private int airlineID;
        private int flightID;
        private string customerName;
        private string customerEmail;
        private string customerPhone;
        private int seatsBooked;
        private int travelSiteID;
        private string travelSiteAPIToken;

        public int AirlineID
        {
            get { return airlineID; }
            set { airlineID = value; }
        }

        public int FlightID
        {
            get { return flightID; }
            set { flightID = value; }
        }

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        public string CustomerEmail
        {
            get { return customerEmail; }
            set { customerEmail = value; }
        }

        public string CustomerPhone
        {
            get { return customerPhone; }
            set { customerPhone = value; }
        }

        public int SeatsBooked
        {
            get { return seatsBooked; }
            set { seatsBooked = value; }
        }

        public int TravelSiteID
        {
            get { return travelSiteID; }
            set { travelSiteID = value; }
        }

        public string TravelSiteAPIToken
        {
            get { return travelSiteAPIToken; }
            set { travelSiteAPIToken = value; }
        }
    }
}
