namespace CarsAPI.Models
{
    public class Car
    {
        public int CarID { get; set; }
        public int AgencyID { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public decimal DailyRate { get; set; }
        public bool Available { get; set; }
        public string PickupLocationCode { get; set; }
        public string DropoffLocationCode { get; set; }
        public string ImagePath { get; set; }
    }
}
