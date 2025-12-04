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

        public async Task<List<CarAPIModel>> FindCarsAsync(string pickup, string dropoff, string carType, decimal minPrice, decimal maxPrice)
        {
            string url =
                "api/RentalCar/FindCars?"
                + "pickup=" + Uri.EscapeDataString(pickup)
                + "&dropoff=" + Uri.EscapeDataString(dropoff)
                + "&carType=" + Uri.EscapeDataString(carType)
                + "&minPrice=" + minPrice
                + "&maxPrice=" + maxPrice;

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Car API error {(int)response.StatusCode}: {content}");

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            List<CarAPIModel> cars = JsonSerializer.Deserialize<List<CarAPIModel>>(content, options);
            return cars ?? new List<CarAPIModel>();
        }

        public async Task<ReservationResponse> ReserveCarAsync(ReservationRequest request)
        {
            string url = "api/RentalCar/Reserve";

            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string json = JsonSerializer.Serialize(request, options);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, data);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Car API error " + (int)response.StatusCode + ": " + content);

            ReservationResponse result = JsonSerializer.Deserialize<ReservationResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }
    }
}
