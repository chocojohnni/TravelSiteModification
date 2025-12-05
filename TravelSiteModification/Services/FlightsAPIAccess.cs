using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TravelSiteModification.Models;

namespace TravelSiteModification.Services

{
    public class FlightsAPIAccess
    {
        private readonly string baseUrl = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tun31378/WebAPI/api/Flights";

        public string GetBaseUrl()
        {
            return baseUrl;
        }

        private readonly HttpClient httpClient;

        public FlightsAPIAccess(HttpClient httpClientParameter)
        {
            httpClient = httpClientParameter;
        }

        /// <summary>
        /// Gets the carriers that operate between a specific route.
        /// </summary>
        public async Task<List<Airline>> GetCarriersAsync(
            string depCity,
            string depState,
            string arrCity,
            string arrState)
        {
            string url =
                $"{baseUrl}/carriers?depCity={depCity}&depState={depState}&arrCity={arrCity}&arrState={arrState}";

            Debug.WriteLine("----- GET CARRIERS -----");
            Debug.WriteLine("URL: " + url);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            string rawJson = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("RAW CARRIER RESPONSE: " + rawJson);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Airline>();
            }

            var apiCarriers = JsonConvert.DeserializeObject<List<dynamic>>(rawJson);
            List<Airline> carriers = new List<Airline>();

            if (apiCarriers == null)
            {
                return carriers;
            }

            foreach (var c in apiCarriers)
            {
                carriers.Add(new Airline
                {
                    AirLineID = (int)c.airCarrierID,
                    AirlineName = (string)c.airCarrierName,
                    Imageurl = (string)c.imageurl
                });
            }

            return carriers;
        }

        /// <summary>
        /// Gets all carriers in the system.
        /// </summary>
        public async Task<List<Airline>> GetAllCarriersAsync()
        {
            string url = $"{baseUrl}/carriers/all";

            Debug.WriteLine("----- GET ALL CARRIERS -----");
            Debug.WriteLine("URL: " + url);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            string rawJson = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("RAW CARRIER RESPONSE: " + rawJson);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Airline>();
            }

            var carriers =
                JsonConvert.DeserializeObject<List<Airline>>(rawJson) ?? new List<Airline>();

            return carriers;
        }

        /// <summary>
        /// Finds flights between two cities with optional requirements (airline, price, nonstop, first-class).
        /// Any null property in FlightRequirements is treated as "no filter" for that field.
        /// </summary>
        public async Task<List<Flight>> FindFlightsAsync(
            string depCity,
            string depState,
            string arrCity,
            string arrState,
            FlightRequirements requirements)
        {
            string url =
                $"{baseUrl}/find?depCity={depCity}&depState={depState}&arrCity={arrCity}&arrState={arrState}";

            if (requirements != null)
            {
                if (requirements.AirlineID != null)
                {
                    url += $"&AirlineID={requirements.AirlineID}";
                }

                if (requirements.MaxPrice != null)
                {
                    url += $"&MaxPrice={requirements.MaxPrice}";
                }

                if (requirements.NonStop != null)
                {
                    url += $"&NonStop={requirements.NonStop.Value.ToString().ToLower()}";
                }

                if (requirements.FirstClass != null)
                {
                    url += $"&FirstClass={requirements.FirstClass.Value.ToString().ToLower()}";
                }
            }

            Debug.WriteLine("----- FIND FLIGHTS -----");
            Debug.WriteLine("URL: " + url);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            string rawJson = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("RAW FIND RESPONSE: " + rawJson);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Flight>();
            }

            var apiFlights = JsonConvert.DeserializeObject<List<dynamic>>(rawJson);
            List<Flight> flights = new List<Flight>();

            if (apiFlights == null)
            {
                return flights;
            }

            foreach (var f in apiFlights)
            {
                flights.Add(new Flight
                {
                    FlightID = (int)f.flightID,
                    AirlineID = (int)f.airCarrierID,
                    DepartureCity = (string)f.departureCity,
                    DepartureState = (string)f.departureState,
                    ArrivalCity = (string)f.arrivalCity,
                    ArrivalState = (string)f.arrivalState,
                    DepartureTime = (DateTime)f.departureTime,
                    ArrivalTime = (DateTime)f.arrivalTime,
                    Price = (decimal)f.seatPrice,
                    SeatsAvailable = (int)f.seatsAvailable,
                    NonStop = (bool)f.nonStop,
                    FirstClass = (bool)f.firstClass
                });
            }

            return flights;
        }

        /// <summary>
        /// Calls the Flights Web API /reserve endpoint to record a reservation.
        /// Returns true if the API call succeeds (2xx status).
        /// </summary>
        public async Task<bool> ReserveFlightAsync(FlightReserveRequest request)
        {
            string url = $"{baseUrl}/reserve";

            string json = JsonConvert.SerializeObject(request);
            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string rawResponse = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("----- RESERVE FLIGHT -----");
            Debug.WriteLine("URL: " + url);
            Debug.WriteLine("REQUEST JSON: " + json);
            Debug.WriteLine("RESPONSE (" + (int)response.StatusCode + "): " + rawResponse);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }

        public async Task<List<FlightImage>> GetFlightImagesAsync(int flightId)
        {
            string url = baseUrl + "/" + flightId.ToString() + "/images";

            Debug.WriteLine("----- GET FLIGHT IMAGES -----");
            Debug.WriteLine("URL: " + url);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            string rawJson = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("RAW FLIGHT IMAGES RESPONSE: " + rawJson);

            if (!response.IsSuccessStatusCode)
            {
                return new List<FlightImage>();
            }

            List<FlightImage> images =
                JsonConvert.DeserializeObject<List<FlightImage>>(rawJson);

            if (images == null)
            {
                images = new List<FlightImage>();
            }

            return images;
        }
    }
}