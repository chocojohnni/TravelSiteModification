namespace TravelSiteModification.Models.Trips
{
    public class FlightBookingItem
    {
        private string airlineName;
        private string departureCity;
        private string arrivalCity;

        private string departureTime;
        private string arrivalTime;

        private string status;
        private string totalPrice;

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

        public string DepartureTime
        {
            get { return departureTime; }
            set { departureTime = value; }
        }

        public string ArrivalTime
        {
            get { return arrivalTime; }
            set { arrivalTime = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
    }
}
