namespace TravelSiteModification.Models
{
    public class CarAPIModel
    {
        private int carID;
        private int agencyID;
        private string carModel;
        private string carType;
        private decimal dailyRate;
        private bool available;
        private string pickupLocationCode;
        private string dropoffLocationCode;
        private string imagePath;

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

        public decimal DailyRate
        {
            get { return dailyRate; }
            set { dailyRate = value; }
        }

        public bool Available
        {
            get { return available; }
            set { available = value; }
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

        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }
    }
}
