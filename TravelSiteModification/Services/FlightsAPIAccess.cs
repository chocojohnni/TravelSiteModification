using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using TravelSiteModification.Models;

namespace TravelSiteModification.Services

{
    public class FlightsAPIAccess
    {
        private readonly string baseUrl = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tun31378/WebAPI/api/Flights";

        public string GetBaseUrl() => baseUrl;

        // GET carriers for a route
        public List<Airline> GetCarriers(string depCity, string depState, string arrCity, string arrState)
        {
            string url =
                $"{baseUrl}/carriers?depCity={depCity}&depState={depState}&arrCity={arrCity}&arrState={arrState}";

            Debug.WriteLine("----- GET CARRIERS -----");
            Debug.WriteLine("URL: " + url);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;

            string rawJson = response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine("RAW CARRIER RESPONSE: " + rawJson);

            if (!response.IsSuccessStatusCode)
                return new List<Airline>();

            var apiCarriers = JsonConvert.DeserializeObject<List<dynamic>>(rawJson);

            List<Airline> carriers = new List<Airline>();

            if (apiCarriers == null)
                return carriers;

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

        public List<Airline> GetAllCarriers()
        {
            string url = $"{baseUrl}/carriers/all";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                    return new List<Airline>();

                var json = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<List<Airline>>(json) ?? new List<Airline>();
            }
        }

        public List<Flight> FindFlights(
            string depCity,
            string depState,
            string arrCity,
            string arrState,
            FlightRequirements requirements)
        {
            string url = $"{baseUrl}/find?depCity={depCity}&depState={depState}&arrCity={arrCity}&arrState={arrState}";

            if (requirements != null)
            {
                if (requirements.AirlineID != null)
                    url += $"&AirlineID={requirements.AirlineID}";
                if (requirements.MaxPrice != null)
                    url += $"&MaxPrice={requirements.MaxPrice}";
                if (requirements.NonStop != null)
                    url += $"&NonStop={requirements.NonStop}";
                if (requirements.FirstClass != null)
                    url += $"&FirstClass={requirements.FirstClass}";
            }

            Debug.WriteLine("----- FIND FLIGHTS -----");
            Debug.WriteLine("URL: " + url);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;

            string rawJson = response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine("RAW FIND RESPONSE: " + rawJson);

            if (!response.IsSuccessStatusCode)
                return new List<Flight>();

            var apiFlights = JsonConvert.DeserializeObject<List<dynamic>>(rawJson);
            List<Flight> flights = new List<Flight>();

            if (apiFlights == null)
                return flights;

            foreach (var f in apiFlights)
            {
                flights.Add(new Flight
                {
                    FlightID = (int)f.flightID,
                    AirlineID = (int)f.airCarrierID,
                    DepartureCity = (string)f.departureCity,
                    ArrivalCity = (string)f.arrivalCity,
                    DepartureTime = (DateTime)f.departureTime,
                    ArrivalTime = (DateTime)f.arrivalTime,
                    Price = (decimal)f.seatPrice,
                    SeatsAvailable = (int)f.seatsAvailable
                });
            }

            return flights;
        }

        // (You also have GetFlights, GetAllFlights, ReserveFlight if you want them later)
    }
}