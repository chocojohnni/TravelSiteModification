using System.Collections.Generic;

namespace TravelSiteModification.Models
{
    public class TicketmasterSearchViewModel
    {
        public string City { get; set; }
        public string StateCode { get; set; }
        public string Keyword { get; set; }

        public List<TicketmasterEventResult> Results { get; set; } = new();
        public string ErrorMessage { get; set; }
        public bool HasSearched { get; set; }
    }
}
