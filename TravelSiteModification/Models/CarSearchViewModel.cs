using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TravelSiteModification.Models
{
    public class CarSearchViewModel
    {
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public string PickupDate { get; set; }
        public string DropoffDate { get; set; }

        public List<SelectListItem> Locations { get; set; } = new();
        public List<CarResultViewModel> Results { get; set; } = new();

        public string ErrorMessage { get; set; }
        public string SearchCriteriaMessage { get; set; }
    }
}
