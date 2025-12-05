namespace TravelSiteModification.Models
{
    public class FlightImage
    {
        private int imageID;
        private int flightID;
        private string imageUrl;
        private string caption;

        public int ImageID
        {
            get { return imageID; }
            set { imageID = value; }
        }

        public int FlightID
        {
            get { return flightID; }
            set { flightID = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
    }
}
