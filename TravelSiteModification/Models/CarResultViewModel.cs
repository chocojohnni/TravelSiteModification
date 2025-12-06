using System;

namespace TravelSiteModification.Models
{
    public class CarResultViewModel
    {
        private int carID;
        private int agencyID;

        private string carModel;
        private string carType;
        private decimal pricePerDay;
        private bool available;
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

        public decimal PricePerDay
        {
            get { return pricePerDay; }
            set { pricePerDay = value; }
        }

        public bool Available
        {
            get { return available; }
            set { available = value; }
        }

        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }
    }
}
