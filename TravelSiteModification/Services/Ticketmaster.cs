using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using TravelSiteModification.Models;

namespace TravelSiteModification.Services
{
    public class TicketmasterService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TicketmasterService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = config["Ticketmaster:ApiKey"];
        }

        public async Task<List<TicketmasterEventResult>> SearchEventsAsync(
            string city,
            string stateCode,
            string keyword)
        {
            var results = new List<TicketmasterEventResult>();

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException(
                    "Ticketmaster API key is not configured. Check appsettings.json.");
            }

            // Build URL with query params
            var baseUrl = "https://app.ticketmaster.com/discovery/v2/events.json";

            var queryParts = new List<string>
            {
                $"apikey={_apiKey}"
            };

            if (!string.IsNullOrWhiteSpace(city))
            {
                queryParts.Add($"city={Uri.EscapeDataString(city)}");
            }

            if (!string.IsNullOrWhiteSpace(stateCode))
            {
                queryParts.Add($"stateCode={Uri.EscapeDataString(stateCode)}");
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryParts.Add($"keyword={Uri.EscapeDataString(keyword)}");
            }

            queryParts.Add("size=50"); // limit results

            string url = baseUrl + "?" + string.Join("&", queryParts);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ticketmaster API error: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();

            var root = JObject.Parse(json);
            var eventsToken = root["_embedded"]?["events"];

            if (eventsToken == null)
            {
                // No events found
                return results;
            }

            foreach (var ev in eventsToken)
            {
                var venue = ev["_embedded"]?["venues"]?[0];
                string dateStr = (string)ev["dates"]?["start"]?["localDate"];

                DateTime? date = null;
                if (DateTime.TryParse(dateStr, out var parsedDate))
                {
                    date = parsedDate;
                }

                results.Add(new TicketmasterEventResult
                {
                    EventId = (string)ev["id"],
                    Name = (string)ev["name"],
                    Url = (string)ev["url"],
                    ImageUrl = (string)ev["images"]?[0]?["url"],
                    Venue = (string)venue?["name"],
                    City = (string)venue?["city"]?["name"],
                    EventDate = date
                });
            }

            return results;
        }
    }
}
