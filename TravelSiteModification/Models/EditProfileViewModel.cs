namespace TravelSiteModification.Models
{
    public class EditProfileViewModel
    {
        // Credentials
        public string Email { get; set; }
        public string Password { get; set; }

        // Contact
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }

        // Payment
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public string CVV { get; set; }
        public string CardHolder { get; set; }

        public string Message { get; set; }
    }
}
