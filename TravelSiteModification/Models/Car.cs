namespace TravelSiteModification.Models
{
    public class Car
    {
        private int carID;

        private int agencyID;
        private string agencyName;

        private string carModel;
        private int year;

        private string pickupLocationCode;
        private string dropoffLocationCode;

        private int seats;
        private string transmission;

        private decimal pricePerDay;
        private decimal totalPrice;

        private string description;

        public int CarID
        {
            get { return carID; }
            set { carID = value; }
        }

        public int AgencyID
        {
            get { return agencyID; }
            set { agencyID = value; }
        }

        public string AgencyName
        {
            get { return agencyName; }
            set { agencyName = value; }
        }

        public string CarModel
        {
            get { return carModel; }
            set { carModel = value; }
        }

        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        public string PickupLocationCode
        {
            get { return pickupLocationCode; }
            set { pickupLocationCode = value; }
        }

        public string DropoffLocationCode
        {
            get { return dropoffLocationCode; }
            set { dropoffLocationCode = value; }
        }

        public int Seats
        {
            get { return seats; }
            set { seats = value; }
        }

        public string Transmission
        {
            get { return transmission; }
            set { transmission = value; }
        }

        public decimal PricePerDay
        {
            get { return pricePerDay; }
            set { pricePerDay = value; }
        }

        public decimal TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }



}
