namespace CarsAPI.Models
{
    public class ReservationRequest
    {
        public int UserID { get; set; }
        public int CarID { get; set; }
        public string PickupDate { get; set; }
        public string DropoffDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int TravelSiteID { get; set; }
        public string TravelSiteAPIToken { get; set; }
    }
}
