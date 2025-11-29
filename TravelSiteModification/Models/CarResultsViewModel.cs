namespace TravelSiteModification.Models
{
    public class CarResultsViewModel
    {
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }

        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }

        public List<CarResultItem> Cars { get; set; } = new();
        public CarDetailViewModel CarDetails { get; set; }

        public bool IsDetailView { get; set; } = false;

        public List<string> Locations { get; set; } = new() { "NYC", "LAX", "MIA", "SEA" };
    }

}
