using Newtonsoft.Json;
namespace TravelSiteModification.Models
{
    public class Airline
    {
        [JsonProperty("airCarrierID")]
        public int AirLineID { get; set; }

        [JsonProperty("airCarrierName")]
        public string AirlineName { get; set; }

        [JsonProperty("imageurl")]
        public string Imageurl { get; set; }
    }
}
