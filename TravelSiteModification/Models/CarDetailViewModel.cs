namespace TravelSiteModification.Models
{
    public class CarDetailViewModel
    {
        public CarResultItem Car { get; set; }
        public List<CarResultItem> OtherCars { get; set; }

        // Agency Details
        public string AgencyName { get; set; }
        public string AgencyPhone { get; set; }
        public string AgencyEmail { get; set; }

        public CarDetailViewModel()
        {
            OtherCars = new List<CarResultItem>();
        }
    }
}
