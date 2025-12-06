using System.Collections.Generic;

namespace TravelSiteModification.Models
{
    public class CarDetailViewModel
    {
        public int CarID { get; set; }
        public int AgencyID { get; set; }

        public string CarModel { get; set; }
        public string CarType { get; set; }
        public decimal PricePerDay { get; set; }

        public string ImagePath { get; set; }

        public string AgencyName { get; set; }
        public string AgencyPhone { get; set; }
        public string AgencyEmail { get; set; }

        public List<CarResultViewModel> OtherAgencyCars { get; set; }
        public List<string> GalleryImages { get; set; }

        public CarDetailViewModel()
        {
            OtherAgencyCars = new List<CarResultViewModel>();
            GalleryImages = new List<string>();
        }
    }
}
