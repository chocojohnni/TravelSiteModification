namespace TravelSiteModification.Models
{
    public class Car
    {
        public int CarID { get; set; }

        public int AgencyID { get; set; }
        public string AgencyName { get; set; }

        public string CarModel { get; set; }
        public int Year { get; set; }

        public string PickupLocationCode { get; set; }
        public string DropoffLocationCode { get; set; }

        public int Seats { get; set; }
        public string Transmission { get; set; }

        public decimal PricePerDay { get; set; }
        public decimal TotalPrice { get; set; }

        public string Description { get; set; }
    }



}
