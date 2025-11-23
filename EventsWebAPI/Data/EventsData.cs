using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

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

        public int Reserve(int eventOfferingId, int qty, string customerName, string customerEmail, string travelSiteId, string token)
        {
            SqlCommand cmd = new SqlCommand("dbo.spEventsReserve");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EventOfferingId", eventOfferingId);
            cmd.Parameters.AddWithValue("@Qty", qty);
            cmd.Parameters.AddWithValue("@CustomerName", customerName);
            cmd.Parameters.AddWithValue("@CustomerEmail", customerEmail);
            cmd.Parameters.AddWithValue("@TravelSiteId", travelSiteId);
            cmd.Parameters.AddWithValue("@TravelSiteApiToken", token);

            object result = db.ExecuteScalarFunction(cmd);
            int reservationId = 0;

            if (result != null)
            {
                return reservationId;
            }

            return 0;
        }
    }
}
