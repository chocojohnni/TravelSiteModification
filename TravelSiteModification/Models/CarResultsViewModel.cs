using System;
using System.Collections.Generic;


namespace TravelSiteModification.Models
{
    public class CarResultsViewModel
    {
        public string? PickupLocation { get; set; }
        public string? DropoffLocation { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DropoffDate { get; set; }


        public List<CarResultItem> Cars { get; set; } = new();


        public bool IsDetailView { get; set; }
        public CarDetailViewModel CarDetails { get; set; }
        public List<CarResultItem> Results { get; set; } = new List<CarResultItem>();
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }
}