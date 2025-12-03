namespace TravelSiteModification.Models
{
    public class ReservationResponse
    {
        public int ReservationID { get; set; }
        public string Message { get; set; }

        public bool Success
        {
            get { return ReservationID > 0; }
        }
    }
}
