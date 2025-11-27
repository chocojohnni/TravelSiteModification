namespace TravelSiteModification.Models
{
    public class CarDetailsViewModel
    {
        public int CarID { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public decimal PricePerDay { get; set; }
        public string ImagePath { get; set; }

        public string AgencyName { get; set; }
        public string AgencyPhone { get; set; }
        public string AgencyEmail { get; set; }

        public List<AgencyCarViewModel> OtherCars { get; set; }
    }

}
