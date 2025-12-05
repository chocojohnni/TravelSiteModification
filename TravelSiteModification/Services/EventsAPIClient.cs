using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TravelSiteModification.Models;

namespace TravelSiteModification.Services
{
    public class EventsAPIClient
    {
        private readonly HttpClient client;

        public EventsAPIClient(HttpClient httpClient)
        {
            client = httpClient;
        }

        public async Task<List<EventActivity>> GetActivitiesAsync(string city, string state)
        {
            string url =
                "api/Event/Activities?city=" +
                Uri.EscapeDataString(city) +
                "&state=" +
                Uri.EscapeDataString(state);

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Events API error " + (int)response.StatusCode + ": " + content);
            }

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            List<EventActivity> events = JsonSerializer.Deserialize<List<EventActivity>>(content, options);

            if (events == null)
            {
                events = new List<EventActivity>();
            }

            return events;
        }

        public async Task<List<EventSeat>> GetSeatsForEventAsync(int eventId)
        {
            string url = "api/Event/Seats?eventId=" + eventId.ToString();

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new List<EventSeat>();
            }

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            List<EventSeat> seats = JsonSerializer.Deserialize<List<EventSeat>>(content, options);

            if (seats == null)
            {
                seats = new List<EventSeat>();
            }

            return seats;
        }
    }
}
