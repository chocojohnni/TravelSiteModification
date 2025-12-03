using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class SpendingSummary
    {
        private decimal hotelsTotal;
        private decimal flightsTotal;
        private decimal carsTotal;
        private decimal eventsTotal;

        public decimal HotelsTotal
        {
            get { return hotelsTotal; }
            set { hotelsTotal = value; }
        }

        public decimal FlightsTotal
        {
            get { return flightsTotal; }
            set { flightsTotal = value; }
        }

        public decimal CarsTotal
        {
            get { return carsTotal; }
            set { carsTotal = value; }
        }

        public decimal EventsTotal
        {
            get { return eventsTotal; }
            set { eventsTotal = value; }
        }

        public decimal GrandTotal
        {
            get { return hotelsTotal + flightsTotal + carsTotal + eventsTotal; }
        }
    }
}


