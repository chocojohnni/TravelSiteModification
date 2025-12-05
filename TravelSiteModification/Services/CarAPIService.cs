using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TravelSiteModification.Models;

namespace TravelSiteModification.Services
{
    public class CarAPIService
    {
        private readonly HttpClient client;

        public CarAPIService(HttpClient httpClient)
        {
            client = httpClient;
        }

        public async Task<List<CarAPIModel>> FindCarsAsync(string city, string carType, decimal minPrice, decimal maxPrice)
        {
            string url =
                $"api/RentalCar/FindCars?city={Uri.EscapeDataString(city)}" +
                $"&carType={Uri.EscapeDataString(carType)}" +
                $"&minPrice={minPrice}" +
                $"&maxPrice={maxPrice}";

            Console.WriteLine("Car API Url: " + url);

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Car API error {(int)response.StatusCode}: {content}");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Console.WriteLine("Car API URL: " + url);
            return JsonSerializer.Deserialize<List<CarAPIModel>>(content, options) ?? new List<CarAPIModel>();
        }

        public async Task<ReservationResponse> ReserveCarAsync(ReservationRequest request)
        {
            string url = "api/RentalCar/Reserve";

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, data);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Car API error {(int)response.StatusCode}: {content}");

            Console.WriteLine("Car API URL: " + url);
            return JsonSerializer.Deserialize<ReservationResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
