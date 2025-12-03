namespace TravelSiteModification.Models
{
    public class CurrentPackageViewModel
    {
        private bool hasPackage;
        private int packageId;
        private string packageName;
        private string status;
        private decimal totalCost;
        private DateTime startDate;
        private DateTime endDate;
        private List<PackageFlightItem> flights;
        private List<PackageHotelItem> hotels;
        private List<PackageCarItem> cars;
        private List<PackageEventItem> events;


        public CurrentPackageViewModel()
        {
            flights = new List<PackageFlightItem>();
            hotels = new List<PackageHotelItem>();
            cars = new List<PackageCarItem>();
            events = new List<PackageEventItem>();
        }

        public bool HasPackage
        {
            get { return hasPackage; }
            set { hasPackage = value; }
        }

        public int PackageId
        {
            get { return packageId; }
            set { packageId = value; }
        }

        public string PackageName
        {
            get { return packageName; }
            set { packageName = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public decimal TotalCost
        {
            get { return totalCost; }
            set { totalCost = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public List<PackageFlightItem> Flights
        {
            get { return flights; }
            set { flights = value; }
        }

        public List<PackageHotelItem> Hotels
        {
            get { return hotels; }
            set { hotels = value; }
        }

        public List<PackageCarItem> Cars
        {
            get { return cars; }
            set { cars = value; }
        }

        public List<PackageEventItem> Events
        {
            get { return events; }
            set { events = value; }
        }
    }
}
