using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using EventsWebAPI.Models;

namespace EventsWebAPI.Data
{
    public class EventsData
    {
        private DBConnect db = new DBConnect();

        public DataSet GetActivityAgencies(string city, string state)
        {
            SqlCommand cmd = new SqlCommand("dbo.TP_spEvents_GetActivityAgencies");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@State", state);
            return db.GetDataSetUsingCmdObj(cmd);
        }

        public DataSet GetActivities(string city, string state)
        {
            SqlCommand cmd = new SqlCommand("dbo.TP_spEvents_GetActivities");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@State", state);
            return db.GetDataSetUsingCmdObj(cmd);
        }

        public DataSet FindActivities(string city, string state, DateTime? fromDate, DateTime? toDate, decimal? minPrice, decimal? maxPrice, string keyword)
        {
            SqlCommand cmd = new SqlCommand("dbo.TP_spEvents_FindActivities");
            cmd.CommandType = CommandType.StoredProcedure;

            if (city == null)
            {
                city = String.Empty;
            }
            if (state == null)
            { 
                state = String.Empty;
            }
            if (keyword == null)
            {
                keyword = String.Empty;
            }

            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@State", state);

            SqlParameter pFrom = new SqlParameter("@FromDate", SqlDbType.Date);
            if (fromDate.HasValue)
            {
                pFrom.Value = fromDate.Value;
            }
            else
            {
                pFrom.Value = DBNull.Value;
                cmd.Parameters.Add(pFrom);
            }

            SqlParameter pTo = new SqlParameter("@ToDate", SqlDbType.Date);
            if (toDate.HasValue)
            {
                pTo.Value = toDate.Value;
            }
            else
            {
                pTo.Value = DBNull.Value;
                cmd.Parameters.Add(pTo);
            }

            SqlParameter pMin = new SqlParameter("@MinPrice", SqlDbType.Decimal);
            pMin.Precision = 18; pMin.Scale = 2;

            if (minPrice.HasValue)
            {
                pMin.Value = minPrice.Value;
            }
            else
            {
                pMin.Value = DBNull.Value;
                cmd.Parameters.Add(pMin);
            }

            SqlParameter pMax = new SqlParameter("@MaxPrice", SqlDbType.Decimal);
            pMax.Precision = 18; pMax.Scale = 2;

            if (maxPrice.HasValue)
            {
                pMax.Value = maxPrice.Value;
            }
            else
            {
                pMax.Value = DBNull.Value;
                cmd.Parameters.Add(pMax);
            }

            cmd.Parameters.AddWithValue("@Keyword", keyword.Length == 0 ? (object)DBNull.Value : keyword);

            return db.GetDataSetUsingCmdObj(cmd);
        }

        public DataSet FindActivitiesByVenue(int venueId, string city, string state, DateTime? fromDate, DateTime? toDate)
        {
            SqlCommand cmd = new SqlCommand("dbo.TP_spFindActivitiesByVenue");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VenueId", venueId);
            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@State", state);

            SqlParameter pFrom = new SqlParameter("@FromDate", SqlDbType.Date);
            if (fromDate.HasValue)
            {
                pFrom.Value = fromDate.Value;
            }
            else
            {
                pFrom.Value = DBNull.Value;
                cmd.Parameters.Add(pFrom);
            }

            SqlParameter pTo = new SqlParameter("@ToDate", SqlDbType.Date);
            if (toDate.HasValue)
            {
                pTo.Value = toDate.Value;
            }
            else
            {
                pTo.Value = DBNull.Value;
                cmd.Parameters.Add(pTo);
            }

            return db.GetDataSetUsingCmdObj(cmd);
        }

        public int Reserve(int eventOfferingId, int qty, string customerName, string customerEmail, int travelSiteId, string travelSiteApiToken)
        {
            SqlCommand cmd = new SqlCommand("dbo.TP_spEventsReserve");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EventOfferingId", eventOfferingId);
            cmd.Parameters.AddWithValue("@Qty", qty);
            cmd.Parameters.AddWithValue("@CustomerName", customerName);
            cmd.Parameters.AddWithValue("@CustomerEmail", customerEmail);
            cmd.Parameters.AddWithValue("@TravelSiteId", travelSiteId);
            cmd.Parameters.AddWithValue("@TravelSiteApiToken", travelSiteApiToken);

            object result = db.ExecuteScalarFunction(cmd);

            int reservationId = 0;
            if (result != null && int.TryParse(result.ToString(), out reservationId))
            {
                return reservationId;
            }

            return 0;
        }

        public List<EventSeat> GetSeatsForEvent(int eventId)
        {
            SqlCommand cmd = new SqlCommand("dbo.TP_GetSeatsForEvent");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EventID", eventId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            List<EventSeat> seats = new List<EventSeat>();

            if (ds != null &&
                ds.Tables.Count > 0 &&
                ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    EventSeat seat = new EventSeat();

                    seat.SeatId = Convert.ToInt32(row["SeatID"]);
                    seat.EventId = Convert.ToInt32(row["EventID"]);

                    if (row["Section"] == DBNull.Value)
                    {
                        seat.Section = String.Empty;
                    }
                    else
                    {
                        seat.Section = row["Section"].ToString();
                    }

                    seat.RowLabel = row["RowLabel"].ToString();
                    seat.SeatNumber = Convert.ToInt32(row["SeatNumber"]);

                    if (row["PriceAdjust"] == DBNull.Value)
                    {
                        seat.PriceAdjust = 0m;
                    }
                    else
                    {
                        seat.PriceAdjust = Convert.ToDecimal(row["PriceAdjust"]);
                    }

                    if (row["IsReserved"] == DBNull.Value)
                    {
                        seat.IsReserved = false;
                    }
                    else
                    {
                        seat.IsReserved = Convert.ToBoolean(row["IsReserved"]);
                    }

                    seats.Add(seat);
                }
            }

            return seats;
        }

        public int ReserveWithSeats(int eventOfferingId, List<int> seatIds, string customerName, string customerEmail, int travelSiteId, string travelSiteApiToken)
        {
            if (seatIds == null || seatIds.Count == 0)
            {
                return 0;
            }

            string csvSeats = string.Join(",", seatIds);

            SqlCommand cmd = new SqlCommand("dbo.TP_spEventsReserve");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EventOfferingId", eventOfferingId);
            cmd.Parameters.AddWithValue("@Qty", seatIds.Count);
            cmd.Parameters.AddWithValue("@CustomerName", customerName);
            cmd.Parameters.AddWithValue("@CustomerEmail", customerEmail);
            cmd.Parameters.AddWithValue("@TravelSiteId", travelSiteId);
            cmd.Parameters.AddWithValue("@TravelSiteApiToken", travelSiteApiToken);
            cmd.Parameters.AddWithValue("@SeatIDs", csvSeats);

            object result = db.ExecuteScalarFunction(cmd);

            int reservationId = 0;
            if (result != null && int.TryParse(result.ToString(), out reservationId))
            {
                return reservationId;
            }

            return 0;
        }
    }
}
