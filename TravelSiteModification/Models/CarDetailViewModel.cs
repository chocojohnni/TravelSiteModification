using System.Collections.Generic;

namespace TravelSiteModification.Models
{
    public class CarDetailViewModel
    {
        private int carID;
        private int agencyID;

        private string carModel;
        private string carType;
        private decimal pricePerDay;

        private string imagePath;

        private string agencyName;
        private string agencyPhone;
        private string agencyEmail;

        private List<CarResultViewModel> otherAgencyCars;
        private List<string> galleryImages;

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

        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        public string AgencyName
        {
            get { return agencyName; }
            set { agencyName = value; }
        }

        public string AgencyPhone
        {
            get { return agencyPhone; }
            set { agencyPhone = value; }
        }

        public string AgencyEmail
        {
            get { return agencyEmail; }
            set { agencyEmail = value; }
        }

        public List<CarResultViewModel> OtherAgencyCars
        {
            get { return otherAgencyCars; }
            set { otherAgencyCars = value; }
        }

        public List<string> GalleryImages
        {
            get { return galleryImages; }
            set { galleryImages = value; }
        }

        public CarDetailViewModel()
        {
            otherAgencyCars = new List<CarResultViewModel>();
            galleryImages = new List<string>();
        }
    }
}
