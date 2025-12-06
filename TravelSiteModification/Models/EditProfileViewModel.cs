namespace TravelSiteModification.Models
{
    public class EditProfileViewModel
    {
        // Credentials
        private string email;
        private string password;

        // Contact
        private string address;
        private string city;
        private string state;
        private string zip;
        private string phone;

        // Payment
        private string cardType;
        private string cardNumber;
        private string expiry;
        private string cvv;
        private string cardHolder;

        private string message;

        // Credentials
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        // Contact
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public string Zip
        {
            get { return zip; }
            set { zip = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        // Payment
        public string CardType
        {
            get { return cardType; }
            set { cardType = value; }
        }

        public string CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        public string Expiry
        {
            get { return expiry; }
            set { expiry = value; }
        }

        public string CVV
        {
            get { return cvv; }
            set { cvv = value; }
        }

        public string CardHolder
        {
            get { return cardHolder; }
            set { cardHolder = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
