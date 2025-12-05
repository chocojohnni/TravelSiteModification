namespace TravelSiteModification.Models
{
    public class EventSeat
    {
        private int seatId;
        private int eventId;
        private string section;
        private string rowLabel;
        private int seatNumber;
        private decimal priceAdjust;
        private bool isReserved;

        public int SeatId
        {
            get { return seatId; }
            set { seatId = value; }
        }

        public int EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

        public string Section
        {
            get { return section; }
            set { section = value; }
        }

        public string RowLabel
        {
            get { return rowLabel; }
            set { rowLabel = value; }
        }

        public int SeatNumber
        {
            get { return seatNumber; }
            set { seatNumber = value; }
        }

        public decimal PriceAdjust
        {
            get { return priceAdjust; }
            set { priceAdjust = value; }
        }

        public bool IsReserved
        {
            get { return isReserved; }
            set { isReserved = value; }
        }

        public string DisplayLabel
        {
            get { return RowLabel + "-" + SeatNumber.ToString(); }
        }
    }
}
