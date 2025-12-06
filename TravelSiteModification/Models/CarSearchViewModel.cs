using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TravelSiteModification.Models
{
    public class CarSearchViewModel
    {
        private string pickupLocation;
        private string dropoffLocation;
        private string pickupDate;
        private string dropoffDate;

        private List<SelectListItem> locations;
        private List<CarResultViewModel> results;

        private string errorMessage;
        private string searchCriteriaMessage;

        public string PickupLocation
        {
            get { return pickupLocation; }
            set { pickupLocation = value; }
        }

        public string DropoffLocation
        {
            get { return dropoffLocation; }
            set { dropoffLocation = value; }
        }

        public string PickupDate
        {
            get { return pickupDate; }
            set { pickupDate = value; }
        }

        public string DropoffDate
        {
            get { return dropoffDate; }
            set { dropoffDate = value; }
        }

        public List<SelectListItem> Locations
        {
            get { return locations; }
            set { locations = value; }
        }

        public List<CarResultViewModel> Results
        {
            get { return results; }
            set { results = value; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public string SearchCriteriaMessage
        {
            get { return searchCriteriaMessage; }
            set { searchCriteriaMessage = value; }
        }

        public CarSearchViewModel()
        {
            pickupLocation = "";
            dropoffLocation = "";
            pickupDate = "";
            dropoffDate = "";

            errorMessage = "";
            searchCriteriaMessage = "";

            locations = new List<SelectListItem>();
            results = new List<CarResultViewModel>();
        }
    }
}
