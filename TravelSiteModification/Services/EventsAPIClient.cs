using System.Text.Json;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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
    }
}
