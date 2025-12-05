using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using EventsWebAPI.Data;
using System.Data;
using EventsWebAPI.Models;

namespace EventsWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private EventsData eventsData = new EventsData();

        /// <summary>
        /// Gets a list of activity agencies that operate in a given city and state.
        /// </summary>
        /// <param name="city">City name (e.g., <c>Philadelphia</c>).</param>
        /// <param name="state">State or region (e.g., <c>PA</c>).</param>
        /// <returns>
        /// <para><b>200 OK</b> with a serialized <see cref="DataSet"/> that includes agency rows:
        /// <c>AgencyId</c>, <c>AgencyName</c>, <c>Phone</c>, <c>Email</c>.</para>
        /// <para><b>500</b> on server error.</para>
        /// </returns>
        /// <remarks>
        /// This endpoint maps to stored procedure <c>spEvents_GetActivityAgencies</c>.
        /// </remarks>
        /// <response code="200">A DataSet containing agencies for the city/state.</response>
        /// <response code="500">Server error with message.</response>
        [HttpGet("Agencies")]
        [Produces("application/xml")]
        public IActionResult GetActivityAgencies([FromQuery] string city, [FromQuery] string state)
        {
            try
            {
                DataSet ds = eventsData.GetActivityAgencies(city, state);
                string xml = ds.GetXml();
                return Content(xml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets activities (events) offered in a given city and state with pricing and availability.
        /// </summary>
        /// <param name="city">City name (e.g., <c>Las Vegas</c>).</param>
        /// <param name="state">State or region (e.g., <c>NV</c>).</param>
        /// <returns>
        /// <para><b>200 OK</b> with a serialized <see cref="DataSet"/> that includes:
        /// <c>EventId</c>, <c>EventName</c>, <c>EventDateTime</c>, <c>Description</c>, 
        /// <c>VenueId</c>, <c>VenueName</c>, <c>AgencyId</c>, <c>AgencyName</c>, 
        /// <c>EventOfferingId</c>, <c>Price</c>, <c>SeatsAvailable</c>.</para>
        /// <para><b>500</b> on server error.</para>
        /// </returns>
        /// <remarks>
        /// Maps to stored procedure <c>spEvents_GetActivities</c>.
        /// </remarks>
        /// <response code="200">A DataSet containing activities and offerings.</response>
        /// <response code="500">Server error with message.</response>
        [HttpGet("Activities")]
        [Produces("application/json")]
        public IActionResult GetActivities([FromQuery] string city, [FromQuery] string state)
        {
            try
            {
                DataSet ds = eventsData.GetActivities(city, state);

                if (ds == null || ds.Tables.Count == 0)
                {
                    List<EventActivity> emptyList = new List<EventActivity>();
                    return Ok(emptyList);
                }

                DataTable table = ds.Tables[0];

                List<EventActivity> list = new List<EventActivity>();

                foreach (DataRow row in table.Rows)
                {
                    EventActivity dto = new EventActivity();

                    dto.EventId = Convert.ToInt32(row["EventID"]);
                    dto.EventName = row["EventName"].ToString();
                    dto.City = row["City"].ToString();

                    if (row["EventDate"] != DBNull.Value)
                    {
                        dto.EventDate = Convert.ToDateTime(row["EventDate"]);
                    }

                    if (row["TicketPrice"] != DBNull.Value)
                    {
                        dto.TicketPrice = Convert.ToDecimal(row["TicketPrice"]);
                    }

                    dto.Description = row["Description"].ToString();
                    dto.ImagePath = row["ImagePath"].ToString();

                    list.Add(dto);
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Finds activities by city/state with optional filters for date range, price range, and keyword.
        /// </summary>
        /// <param name="city">City name.</param>
        /// <param name="state">State or region.</param>
        /// <param name="fromDate">Optional start date filter (inclusive).</param>
        /// <param name="toDate">Optional end date filter (inclusive).</param>
        /// <param name="minPrice">Optional minimum price filter.</param>
        /// <param name="maxPrice">Optional maximum price filter.</param>
        /// <param name="keyword">Optional keyword for event name/description match.</param>
        /// <returns>
        /// <para><b>200 OK</b> with a serialized <see cref="DataSet"/> of matching events and offerings.</para>
        /// <para><b>500</b> on server error.</para>
        /// </returns>
        /// <remarks>
        /// Maps to stored procedure <c>spEvents_FindActivities</c>.
        /// </remarks>
        /// <response code="200">A DataSet of filtered activities.</response>
        /// <response code="500">Server error with message.</response>
        [HttpGet("FindActivities")]
        public IActionResult FindActivities([FromQuery] string city, [FromQuery] string state, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] string? keyword)
        {
            try
            {
                DataSet ds = eventsData.FindActivities(city, state, fromDate, toDate, minPrice, maxPrice, keyword);
                string xml = ds.GetXml();
                return Content(xml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Finds activities offered at a specific venue, with optional date range filters.
        /// </summary>
        /// <param name="venueId">Unique ID of the venue.</param>
        /// <param name="city">City name.</param>
        /// <param name="state">State or region.</param>
        /// <param name="fromDate">Optional start date filter (inclusive).</param>
        /// <param name="toDate">Optional end date filter (inclusive).</param>
        /// <returns>
        /// <para><b>200 OK</b> with a serialized <see cref="DataSet"/> of events at the given venue.</para>
        /// <para><b>500</b> on server error.</para>
        /// </returns>
        /// <remarks>
        /// Maps to stored procedure <c>spFindActivitiesByVenue</c>.
        /// </remarks>
        /// <response code="200">A DataSet of venue-filtered activities.</response>
        /// <response code="500">Server error with message.</response>
        [HttpGet("FindByVenue")]
        [Produces("application/xml")]
        public IActionResult FindActivitiesByVenue([FromQuery] int venueId, [FromQuery] string city, [FromQuery] string state, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                DataSet ds = eventsData.FindActivitiesByVenue(venueId, city, state, fromDate, toDate);
                string xml = ds.GetXml();
                return Content(xml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Creates a reservation for a specific event offering.
        /// </summary>
        /// <param name="dto">Reservation details including offering, quantity, customer info, and authentication token.</param>
        /// <returns>
        /// <para><b>200 OK</b> with a JSON object: <c>{ "ReservationId": 123 }</c> on success.</para>
        /// <para><b>400</b> if the request body is missing or invalid.</para>
        /// <para><b>500</b> if the reservation fails or a server error occurs.</para>
        /// </returns>
        /// <remarks>
        /// Requires a valid TravelSiteId and Token that match an active record in TP_TravelSites.
        /// Maps to stored procedure <c>TP_EventsReserve</c>.
        /// </remarks>
        /// <response code="200">Reservation created successfully; returns ReservationId.</response>
        /// <response code="400">Bad request (e.g., missing or invalid body).</response>
        /// <response code="500">Reservation failed or server error.</response>
        [HttpPost("Reserve")]
        [Produces("application/xml")]
        public IActionResult Reserve([FromBody] ReserveDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Missing request body.");
                }

                int reservationId = eventsData.Reserve(dto.EventOfferingId, dto.Qty, dto.CustomerName, dto.CustomerEmail, dto.TravelSiteId, dto.Token);

                if (reservationId <= 0)
                {
                    return StatusCode(500, "Reservation failed.");
                }

                return Ok(new { ReservationId = reservationId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the seating chart for a specific event.
        /// </summary>
        /// <param name="eventId">EventID from EventAttraction.</param>
        [HttpGet("Seats")]
        [Produces("application/json")]
        public IActionResult GetSeats(int eventId)
        {
            try
            {
                if (eventId <= 0)
                {
                    return BadRequest("eventId is required and must be positive.");
                }

                List<EventSeatDto> seats = eventsData.GetSeatsForEvent(eventId);

                return Ok(seats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Creates a reservation for a specific event with explicit seat selection.
        /// </summary>
        /// <remarks>
        /// This is additive; the existing /api/Event/Reserve endpoint still works
        /// for non-seat-based reservations.
        /// </remarks>
        [HttpPost("ReserveWithSeats")]
        [Produces("application/json")]
        public IActionResult ReserveWithSeats([FromBody] ReserveWithSeatsDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Missing request body.");
                }

                if (dto.EventOfferingId <= 0)
                {
                    return BadRequest("EventOfferingId must be provided.");
                }

                if (dto.SeatIds == null || dto.SeatIds.Count == 0)
                {
                    return BadRequest("At least one seat must be selected.");
                }

                int reservationId = eventsData.ReserveWithSeats(
                    dto.EventOfferingId,
                    dto.SeatIds,
                    dto.CustomerName,
                    dto.CustomerEmail,
                    dto.TravelSiteId,
                    dto.Token);

                if (reservationId <= 0)
                {
                    return StatusCode(500, "Reservation failed.");
                }

                return Ok(new { ReservationId = reservationId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }
    }
}
