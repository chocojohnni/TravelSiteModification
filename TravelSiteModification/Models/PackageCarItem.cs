namespace TravelSiteModification.Models
{
    public class PackageCarItem
    {
        private int packageCarId;
        private int carId;
        private string carModel;
        private string carType;
        private string pickupLocationCode;
        private string dropoffLocationCode;
        private decimal pricePerDay;
        private string agencyName;

        public int PackageCarId
        {
            get { return packageCarId; }
            set { packageCarId = value; }
        }

        public int CarId
        {
            get { return carId; }
            set { carId = value; }
        }

        public string CarModel
        {
            get { return carModel; }
            set { carModel = value; }
        }

        public string CarType
        {
            get { return carType; }
            set { carType = value; }
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

        public decimal PricePerDay
        {
            get { return pricePerDay; }
            set { pricePerDay = value; }
        }

        public string AgencyName
        {
            get { return agencyName; }
            set { agencyName = value; }
        }
    }
}
