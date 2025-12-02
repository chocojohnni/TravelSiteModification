using System;
using System.Collections.Generic;

namespace TravelSiteModification.Models
{
    public class HotelDetailsViewModel
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string ImagePath { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        public List<RoomViewModel> Rooms { get; set; }

        public List<string> GalleryImages { get; set; }

        public HotelDetailsViewModel()
        {
            Rooms = new List<RoomViewModel>();
            GalleryImages = new List<string>();
        }
    }
}
