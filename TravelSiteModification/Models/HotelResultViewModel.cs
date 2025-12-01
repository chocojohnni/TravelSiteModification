using System;

namespace TravelSiteModification.Models
{
    public class HotelResultViewModel
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public decimal? Rating { get; set; }
        public decimal PricePerNight { get; set; }
        public string ImagePath { get; set; }
    }
}