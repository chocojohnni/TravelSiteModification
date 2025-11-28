using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Net;
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

        public async Task<List<FlightDto>> FindFlightsAsync(FlightSearchRequest request)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            queryParams.Add("depCity", request.DepCity);
            queryParams.Add("depState", request.DepState);
            queryParams.Add("arrCity", request.ArrCity);
            queryParams.Add("arrState", request.ArrState);

            queryParams.Add("AirlineID", request.AirlineID.ToString(CultureInfo.InvariantCulture));
            queryParams.Add("MaxPrice", request.MaxPrice.ToString(CultureInfo.InvariantCulture));
            queryParams.Add("NonStop", request.NonStop.ToString().ToLowerInvariant());
            queryParams.Add("FirstClass", request.FirstClass.ToString().ToLowerInvariant());

            string basePath = "api/Flights/find";
            string urlWithQuery = QueryHelpers.AddQueryString(basePath, queryParams);

            HttpResponseMessage response = await httpClient.GetAsync(urlWithQuery);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<FlightDto>();
            }

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            List<FlightDto> flights =
                JsonSerializer.Deserialize<List<FlightDto>>(json, jsonOptions);

            if (flights == null)
            {
                flights = new List<FlightDto>();
            }

            return flights;
        }
    }
}
