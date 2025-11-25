namespace TravelSiteModification.Models
{
    public class RoomViewModel
    {
        private int id;
        private string roomName;
        private decimal price;
        private int maxOccupancy;
        private string description;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        public int MaxOccupancy
        {
            get { return maxOccupancy; }
            set { maxOccupancy = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
