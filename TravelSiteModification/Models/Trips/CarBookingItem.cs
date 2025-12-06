namespace TravelSiteModification.Models.Trips
{
    public class CarBookingItem
    {
        private string carModel;
        private string agencyName;
        private string pickupLocationCode;
        private string dropoffLocationCode;

        private string pickupDate;
        private string dropoffDate;

        private string status;
        private string totalPrice;

        public string CarModel
        {
            get { return carModel; }
            set { carModel = value; }
        }

        public string AgencyName
        {
            get { return agencyName; }
            set { agencyName = value; }
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

        public string PickupDate
        {
            get { return pickupDate; }
            set { pickupDate = value; }
        }

        public string DropoffDate
        {
            get { return dropoffDate; }
            set { dropoffDate = value; }
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
