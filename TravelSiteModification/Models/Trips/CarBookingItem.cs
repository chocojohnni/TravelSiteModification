namespace TravelSiteModification.Models.Trips
{
    public class CarBookingItem
    {
        public string CarModel { get; set; }
        public string AgencyName { get; set; }
        public string PickupLocationCode { get; set; }
        public string DropoffLocationCode { get; set; }

        public string PickupDate { get; set; }
        public string DropoffDate { get; set; }

        public string Status { get; set; }
        public string TotalPrice { get; set; }
    }
}
