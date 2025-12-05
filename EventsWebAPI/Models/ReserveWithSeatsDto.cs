using System.Text.Json.Serialization;

namespace EventsWebAPI.Models
{
    [Serializable]
    public class ReserveWithSeatsDto
    {
        private int eventOfferingId;
        private List<int> seatIds;
        private string customerName;
        private string customerEmail;
        private int travelSiteId;
        private string token;

        public int EventOfferingId
        {
            get { return eventOfferingId; }
            set { eventOfferingId = value; }
        }

        public List<int> SeatIds
        {
            get { return seatIds; }
            set { seatIds = value; }
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

        public int TravelSiteId
        {
            get { return travelSiteId; }
            set { travelSiteId = value; }
        }

        public string Token
        {
            get { return token; }
            set { token = value; }
        }
    }
}
