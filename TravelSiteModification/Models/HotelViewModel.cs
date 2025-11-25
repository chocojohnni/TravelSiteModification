namespace TravelSiteModification.Models
{
    public class HotelViewModel
    {
        private int id;
        private string name;
        private string imgPath;
        private string address;
        private string phone;
        private string email;
        private string description;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ImagePath
        {
            get { return imgPath; }
            set { imgPath = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
