using System;

namespace TravelSiteModification.Models
{
    public class RoomViewModel
    {
        public int RoomID { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    }
}