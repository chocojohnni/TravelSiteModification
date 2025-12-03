using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class SpendingData
    {
        public SpendingSummary GetUserSpendingSummary(int userId)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_GetUserSpendingSummary");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            SpendingSummary summary = new SpendingSummary();

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable table = ds.Tables[0];

                    if (table.Rows.Count > 0)
                    {
                        DataRow row = table.Rows[0];

                        // HOTELS TOTAL
                        if (row["HotelsTotal"] != DBNull.Value)
                        {
                            summary.HotelsTotal = Convert.ToDecimal(row["HotelsTotal"]);
                        }
                        else
                        {
                            summary.HotelsTotal = 0m;
                        }

                        // FLIGHTS TOTAL
                        if (row["FlightsTotal"] != DBNull.Value)
                        {
                            summary.FlightsTotal = Convert.ToDecimal(row["FlightsTotal"]);
                        }
                        else
                        {
                            summary.FlightsTotal = 0m;
                        }

                        // CARS TOTAL
                        if (row["CarsTotal"] != DBNull.Value)
                        {
                            summary.CarsTotal = Convert.ToDecimal(row["CarsTotal"]);
                        }
                        else
                        {
                            summary.CarsTotal = 0m;
                        }

                        // EVENTS TOTAL
                        if (row["EventsTotal"] != DBNull.Value)
                        {
                            summary.EventsTotal = Convert.ToDecimal(row["EventsTotal"]);
                        }
                        else
                        {
                            summary.EventsTotal = 0m;
                        }
                    }
                    else
                    {
                        summary.HotelsTotal = 0m;
                        summary.FlightsTotal = 0m;
                        summary.CarsTotal = 0m;
                        summary.EventsTotal = 0m;
                    }
                }
                else
                {
                    summary.HotelsTotal = 0m;
                    summary.FlightsTotal = 0m;
                    summary.CarsTotal = 0m;
                    summary.EventsTotal = 0m;
                }
            }
            else
            {
                summary.HotelsTotal = 0m;
                summary.FlightsTotal = 0m;
                summary.CarsTotal = 0m;
                summary.EventsTotal = 0m;
            }

            return summary;
        }
    }
}


