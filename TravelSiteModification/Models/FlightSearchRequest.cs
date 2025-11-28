namespace TravelSiteModification.Models
{
    public class FlightSearchRequest
    {
        private string depCity;
        private string depState;
        private string arrCity;
        private string arrState;
        private int airlineID;
        private decimal maxPrice;
        private bool nonStop;
        private bool firstClass;

        public string DepCity
        {
            get { return depCity; }
            set { depCity = value; }
        }

        public string DepState
        {
            get { return depState; }
            set { depState = value; }
        }

        public string ArrCity
        {
            get { return arrCity; }
            set { arrCity = value; }
        }

        public string ArrState
        {
            get { return arrState; }
            set { arrState = value; }
        }

        public int AirlineID
        {
            get { return airlineID; }
            set { airlineID = value; }
        }

        public decimal MaxPrice
        {
            get { return maxPrice; }
            set { maxPrice = value; }
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
    }
}
