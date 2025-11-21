using System.ComponentModel.DataAnnotations;

namespace TravelSiteModification.Models
{
    public class EventBookingViewModel
    {
        private int eventId;
        private decimal price;

        private string firstName;
        private string lastName;
        private string email;
        private int ticketCount;

        public int EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        [Required]
        [EmailAddress]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        [Range(1, 100, ErrorMessage = "You must book at least 1 ticket.")]
        [Display(Name = "Number of Tickets")]
        public int TicketCount
        {
            get { return ticketCount; }
            set { ticketCount = value; }
        }


    }
}