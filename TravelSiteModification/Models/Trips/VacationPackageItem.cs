namespace TravelSiteModification.Models.Trips
{
    public class VacationPackageItem
    {
        private string packageName;
        private string status;
        private string totalCost;

        private string startDate;
        private string endDate;

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

        public string TotalCost
        {
            get { return totalCost; }
            set { totalCost = value; }
        }

        public string StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public string EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
    }
}
