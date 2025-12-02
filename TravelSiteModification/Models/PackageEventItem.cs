namespace TravelSiteModification.Models
{
    public class PackageEventItem
    {
        private int packageEventId;
        private int eventId;
        private string eventName;
        private string eventLocation;
        private DateTime eventDate;
        private decimal price;

        public int PackageEventId
        {
            get { return packageEventId; }
            set { packageEventId = value; }
        }

        public int EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

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

        public DateTime EventDate
        {
            get { return eventDate; }
            set { eventDate = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
