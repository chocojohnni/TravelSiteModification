namespace TravelSiteModification.Models
{
    public class CarResultViewModel
    {
        public int CarID { get; set; }
        public int AgencyID { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public decimal PricePerDay { get; set; }
        public string ImagePath { get; set; }
    }

}
