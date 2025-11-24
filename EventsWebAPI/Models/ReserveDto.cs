namespace EventsWebAPI.Models
{
    /// <summary>
    /// Represents a reservation request for an event or activity 
    /// made through the Events Web API.
    /// </summary>
    /// <remarks>
    /// This data transfer object (DTO) is used when a travel site 
    /// or external client submits a reservation request for a specific 
    /// event offering. The object contains the necessary details to 
    /// create a new record in the EventReservations table after 
    /// successful authentication.
    /// </remarks>
    [Serializable]
    public class ReserveDto
    {
        private int eventOfferingId;
        private int qty;
        private string customerName;
        private string customerEmail;
        private int travelSiteId;
        private string token;

        public ReserveDto() { 
        // Make sure this takes the API Token
        }

        /// <summary>
        /// The unique identifier for the event offering being reserved.
        /// </summary>
        /// <example>101</example>
        public int EventOfferingId
        {
            get { return eventOfferingId; }
            set { eventOfferingId = value; }
        }

        /// <summary>
        /// The quantity of tickets or spots being reserved for the event.
        /// </summary>
        /// <example>2</example>
        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        /// <summary>
        /// The full name of the customer making the reservation.
        /// </summary>
        /// <example>Honz Tarriela</example>
        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        /// <summary>
        /// The email address of the customer making the reservation.
        /// </summary>
        /// <remarks>
        /// Used by the system to confirm the reservation and 
        /// provide additional event information.
        /// </remarks>
        /// <example>honztarriela@example.com</example>
        public string CustomerEmail
        {
            get { return customerEmail; }
            set { customerEmail = value; }
        }

        /// <summary>
        /// The unique identifier assigned to the external travel site 
        /// making the API request.
        /// </summary>
        /// <remarks>
        /// This value is used to authenticate the calling application 
        /// and must match a record in the ApiClients table.
        /// </remarks>
        /// <example>TravelSite</example>
        public int TravelSiteId
        {
            get { return travelSiteId; }
            set { travelSiteId = value; }
        }


        /// <summary>
        /// The security token provided to the partner travel site for 
        /// API authentication and authorization.
        /// </summary>
        /// <remarks>
        /// This token must match the TravelSiteApiToken associated with 
        /// the TravelSiteId in the database.
        /// </remarks>
        public string Token
        {
            get { return token; }
            set { token = value; }
        }
    }
}
