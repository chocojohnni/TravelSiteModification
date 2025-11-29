namespace TravelSiteModification.Models
{
    public class CarSearchViewModel
    {
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public DateTime PickupDateString { get; set; }
        public DateTime DropoffDateString { get; set; }

        public List<string> Locations { get; set; } = new List<string>();
        public List<CarResultsViewModel> Results { get; set; } = new List<CarResultsViewModel>();
        public string ErrorMessage { get; set; }

        public string Message { get; set; }
    }

}
