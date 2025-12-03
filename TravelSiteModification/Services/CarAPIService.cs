using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TravelSiteModification.Models;

namespace TravelSiteModification.Services
{
    public class CarAPIService
    {
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:7116/api/RentalCar";

        private const int TravelSiteID = 1;
        private const string TravelSiteAPIToken = "TEST-TOKEN-123";

        public CarAPIService(HttpClient httpClient)
        {
            client = httpClient;
        }

        private static readonly JsonSerializerOptions options =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        // --------------------------
        // Get Cars by Agency (GET)
        // --------------------------
        public async Task<List<CarApiModel>> GetCarsByAgencyAsync(int agencyId, string city)
        {
            string url =
                $"{baseUrl}/GetCarsByAgency?" +
                $"agencyID={agencyId}&" +
                $"city={Uri.EscapeDataString(city)}";

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Car API error {(int)response.StatusCode}: {content}");
            }

            List<CarApiModel>? cars =
                JsonSerializer.Deserialize<List<CarApiModel>>(content, options);

            return cars ?? new List<CarApiModel>();
        }

        // --------------------------
        // Find Cars (GET)
        // --------------------------
        public async Task<List<CarApiModel>> FindCarsAsync(
            string city,
            string carType,
            decimal minPrice,
            decimal maxPrice)
        {
            string url =
                $"{baseUrl}/FindCars?" +
                $"city={Uri.EscapeDataString(city)}&" +
                $"carType={Uri.EscapeDataString(carType)}&" +
                $"minPrice={minPrice}&" +
                $"maxPrice={maxPrice}";

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Car API error {(int)response.StatusCode}: {content}");
            }

            List<CarApiModel>? list =
                JsonSerializer.Deserialize<List<CarApiModel>>(content, options);

            return list ?? new List<CarApiModel>();
        }

        // --------------------------
        // Reserve Car (POST)
        // --------------------------
        public async Task<ReservationResponse?> ReserveCarAsync(ReservationRequest request)
        {
            // Add API required fields
            request.TravelSiteID = TravelSiteID;
            request.TravelSiteAPIToken = TravelSiteAPIToken;

            string jsonBody = JsonSerializer.Serialize(request);
            var body = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/Reserve", body);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Car API error {(int)response.StatusCode}: {content}");
            }

            ReservationResponse? result =
                JsonSerializer.Deserialize<ReservationResponse>(content, options);

            return result;
        }
    }
}
