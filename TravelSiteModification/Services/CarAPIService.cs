using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
                "api/RentalCar/FindCars?pickup=" +
                Uri.EscapeDataString(pickup) +
                "&dropoff=" +
                Uri.EscapeDataString(dropoff) +
                "&carType=" +
                Uri.EscapeDataString(carType == null ? "" : carType) +
                "&minPrice=" +
                minPrice.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "&maxPrice=" +
                maxPrice.ToString(System.Globalization.CultureInfo.InvariantCulture);

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Car API error " + (int)response.StatusCode + ": " + content);
            }

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            List<CarAPIModel> cars = JsonSerializer.Deserialize<List<CarAPIModel>>(content, options);

            if (cars == null)
            {
                cars = new List<CarAPIModel>();
            }

            return cars;
        }

        public async Task<List<CarAPIModel>> GetCarsByAgencyAsync(int agencyId, string pickup, string dropoff)
        {
            string url =
                "api/RentalCar/FindCarsByAgency?agencyId=" +
                agencyId.ToString() +
                "&pickup=" +
                Uri.EscapeDataString(pickup) +
                "&dropoff=" +
                Uri.EscapeDataString(dropoff) +
                "&carType=&minPrice=0&maxPrice=10000";

            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Car API error " + (int)response.StatusCode + ": " + content);
            }

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            List<CarAPIModel> cars = JsonSerializer.Deserialize<List<CarAPIModel>>(content, options);

            if (cars == null)
            {
                cars = new List<CarAPIModel>();
            }

            return cars;
        }

        public async Task<ReservationResponse> ReserveCarAsync(ReservationRequest request)
        {
            string json = JsonSerializer.Serialize(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("api/RentalCar/Reserve", content);
            string body = await response.Content.ReadAsStringAsync();

            ReservationResponse result = new ReservationResponse();
            result.Success = false;
            result.Message = "";
            result.ReservationID = 0;

            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                result.Message = "Car API error " + (int)response.StatusCode + ": " + body;
                return result;
            }

            JsonDocument doc = JsonDocument.Parse(body);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("ReservationID", out JsonElement resIdElement))
            {
                int reservationId = resIdElement.GetInt32();
                result.ReservationID = reservationId;
                if (reservationId > 0)
                {
                    result.Success = true;
                    result.Message = "Reservation successful.";
                }
                else
                {
                    result.Success = false;
                    result.Message = "Reservation failed.";
                }
            }
            else if (root.TryGetProperty("Message", out JsonElement msgElement))
            {
                result.Success = false;
                result.Message = msgElement.GetString();
            }
            else
            {
                result.Success = false;
                result.Message = "Unknown response from Car API.";
            }

            return result;
        }
    }
}
