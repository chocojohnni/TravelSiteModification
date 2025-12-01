using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TravelSiteModification.Models
{
    public class HotelSearchViewModel
    {
        public string Destination { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        public List<SelectListItem> Destinations { get; set; }
        public List<HotelResultViewModel> Results { get; set; }

        public string ErrorMessage { get; set; }
        public string SearchCriteriaMessage { get; set; }

        public HotelSearchViewModel()
        {
            Destinations = new List<SelectListItem>();
            Results = new List<HotelResultViewModel>();
        }
    }
}