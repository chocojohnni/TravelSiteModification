namespace EventsWebAPI.Models
{
    public class EventActivity
    {
        private int eventId;
        private string eventName;
        private string city;
        private DateTime eventDate;
        private decimal ticketPrice;
        private string description;
        private string imagePath;

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

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public DateTime EventDate
        {
            get { return eventDate; }
            set { eventDate = value; }
        }

        public decimal TicketPrice
        {
            get { return ticketPrice; }
            set { ticketPrice = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }
    }
}
