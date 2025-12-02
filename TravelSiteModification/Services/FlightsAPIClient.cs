using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using TravelSiteModification.Models;

namespace TravelSiteModification.Services

{
    public class FlightsAPIClient
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public FlightsAPIClient(HttpClient client)
        {
            httpClient = client;

            jsonOptions = new JsonSerializerOptions();
            jsonOptions.PropertyNameCaseInsensitive = true;
        }

        /// <summary>
        /// Calls the Airline Web API GetFlights endpoint and returns a list of FlightDto
        /// based on the user's search criteria.
        /// </summary>
        public async Task<List<FlightDto>> FindFlightsAsync(FlightSearchRequest request)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("DepartureCity", request.DepCity);
            queryParams.Add("DepartureState", request.DepState);
            queryParams.Add("ArrivalCity", request.ArrCity);
            queryParams.Add("ArrivalState", request.ArrState);

            string relativePath = "airlineservice/airline/GetFlights/";
            string urlWithQuery = QueryHelpers.AddQueryString(relativePath, queryParams);

            AirlineApiAirline airline = new AirlineApiAirline();
            airline.airlineID = request.AirlineID;

            airline.airlineName = "TravelSite Temporary Airline";
            airline.airlineDescription = "Placeholder airline description for flight search.";
            airline.airlineHeadquarters = "N/A";
            airline.airlinePhoneNumber = "000-000-0000";
            airline.airlineEmail = "placeholder@travelsite.example";
            airline.airlineLogo = "https://example.com/logo.png";
            airline.airlineWebsite = "https://example.com";

            string jsonBody = JsonSerializer.Serialize(airline, jsonOptions);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, urlWithQuery);
            httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();

                string statusCodeNumber =
                    ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture);

                string message =
                    "Airline API error " +
                    statusCodeNumber + " " +
                    response.StatusCode.ToString() + ": " +
                    errorContent;

                throw new ApplicationException(message);
            }

            string responseJson = await response.Content.ReadAsStringAsync();

            // Deserialize into the Airline API flight DTOs
            List<AirlineApiFlight> apiFlights =
                JsonSerializer.Deserialize<List<AirlineApiFlight>>(responseJson, jsonOptions);

            if (apiFlights == null)
            {
                apiFlights = new List<AirlineApiFlight>();
            }

            // Map into existing FlightDto class used by the MVC app
            List<FlightDto> flights = new List<FlightDto>();

            for (int i = 0; i < apiFlights.Count; i++)
            {
                AirlineApiFlight apiFlight = apiFlights[i];
                FlightDto dto = new FlightDto();

                dto.FlightID = apiFlight.flightID;
                dto.AirCarrierID = apiFlight.airlineID;

                dto.DepartureCity = apiFlight.departureCity;
                dto.DepartureState = apiFlight.departureState;
                dto.ArrivalCity = apiFlight.arrivalCity;
                dto.ArrivalState = apiFlight.arrivalState;

                DateTime departureTimeParsed;
                if (DateTime.TryParse(
                    apiFlight.departureTime,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out departureTimeParsed))
                {
                    dto.DepartureTime = departureTimeParsed;
                }

                DateTime arrivalTimeParsed;
                if (DateTime.TryParse(
                    apiFlight.arrivalTime,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out arrivalTimeParsed))
                {
                    dto.ArrivalTime = arrivalTimeParsed;
                }

                dto.SeatPrice = Convert.ToDecimal(
                    apiFlight.flightPrice,
                    CultureInfo.InvariantCulture);

                if (apiFlight.atMaxOccupancy)
                {
                    dto.SeatsAvailable = 0;
                }
                else
                {
                    dto.SeatsAvailable = apiFlight.maxOccupancy;
                }

                dto.NonStop = request.NonStop;
                dto.FirstClass = request.FirstClass;

                flights.Add(dto);
            }

            return flights;
        }
    }
}
