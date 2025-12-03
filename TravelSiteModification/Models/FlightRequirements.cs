namespace TravelSiteModification.Models
{
    public class FlightRequirements
    {
        private int? airlineID;
        private decimal? maxPrice;
        private bool? nonStop;
        private bool? firstClass;

        public int? AirlineID
        {
            get { return airlineID; }
            set { airlineID = value; }
        }

        public decimal? MaxPrice
        {
            get { return maxPrice; }
            set { maxPrice = value; }
        }

        public bool? NonStop
        {
            get { return nonStop; }
            set { nonStop = value; }
        }

        public bool? FirstClass
        {
            get { return firstClass; }
            set { firstClass = value; }
        }
    }
}
