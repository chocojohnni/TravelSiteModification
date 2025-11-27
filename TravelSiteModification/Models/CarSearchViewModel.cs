namespace TravelSiteModification.Models
{
    public class CarSearchViewModel
    {
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }

        public List<string> Locations { get; set; } = new List<string>();
        public List<CarResultViewModel> Results { get; set; } = new List<CarResultViewModel>();
        public string ErrorMessage { get; set; }
    }

}
