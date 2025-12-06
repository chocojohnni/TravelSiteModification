using System;
using System.Collections.Generic;


namespace TravelSiteModification.Models
{
    public class CarResultsViewModel
    {
        private string pickupLocation;
        private string dropoffLocation;
        private DateTime pickupDate;
        private DateTime dropoffDate;

        private List<CarResultItem> cars;
        private bool isDetailView;
        private CarDetailViewModel carDetails;
        private List<CarResultItem> results;
        private string message;
        private string errorMessage;

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

        public DateTime PickupDate
        {
            get { return pickupDate; }
            set { pickupDate = value; }
        }

        public DateTime DropoffDate
        {
            get { return dropoffDate; }
            set { dropoffDate = value; }
        }

        public List<CarResultItem> Cars
        {
            get { return cars; }
            set { cars = value; }
        }

        public bool IsDetailView
        {
            get { return isDetailView; }
            set { isDetailView = value; }
        }

        public CarDetailViewModel CarDetails
        {
            get { return carDetails; }
            set { carDetails = value; }
        }

        public List<CarResultItem> Results
        {
            get { return results; }
            set { results = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public CarResultsViewModel()
        {
            cars = new List<CarResultItem>();
            results = new List<CarResultItem>();
            carDetails = new CarDetailViewModel();

            // Initialize string fields to avoid nulls without using '?'
            pickupLocation = "";
            dropoffLocation = "";
            message = "";
            errorMessage = "";
        }
    }
}