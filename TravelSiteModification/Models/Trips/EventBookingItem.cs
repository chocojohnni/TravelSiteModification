namespace TravelSiteModification.Models.Trips
{
    public class EventBookingItem
    {
        private string eventName;
        private string eventLocation;
        private string eventDate;

        private string status;
        private string totalPrice;

        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }

        public string EventLocation
        {
            get { return eventLocation; }
            set { eventLocation = value; }
        }

        public string EventDate
        {
            get { return eventDate; }
            set { eventDate = value; }
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
