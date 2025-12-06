namespace TravelSiteModification.Models
{
    public class AgencyCarViewModel
    {
        private int carID;
        private string carModel;
        private string carType;
        private decimal pricePerDay;
        private string imagePath;

        public int CarID
        {
            get { return carID; }
            set { carID = value; }
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

        public decimal PricePerDay
        {
            get { return pricePerDay; }
            set { pricePerDay = value; }
        }

        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }
    }

}
