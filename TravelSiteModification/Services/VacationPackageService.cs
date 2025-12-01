using System.Data;
using Utilities;
using System.Data.SqlClient;

namespace TravelSiteModification.Services
{
    public class VacationPackageService
    {
        private readonly DBConnect db;
        public VacationPackageService()
        {
            db = new DBConnect();
        }

        /// <summary>
        /// Returns the current "Building" package for this user.
        /// If none exists, creates a new package and returns its ID.
        /// </summary>
        public int EnsureOpenPackage(int userId, string packageName, DateTime startDate, DateTime endDate)
        {
            int packageId = 0;

            // 1. Try to get existing "Building" package (TP_GetVacationPackage)
            SqlCommand getCmd = new SqlCommand();
            getCmd.CommandType = CommandType.StoredProcedure;
            getCmd.CommandText = "TP_GetVacationPackage";
            getCmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(getCmd);

            if (ds != null &&
                ds.Tables.Count > 0 &&
                ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                packageId = Convert.ToInt32(row["PackageID"]);
            }
            else
            {
                // 2. Create a new package (InsertVacationPackage)
                SqlCommand insertCmd = new SqlCommand();
                insertCmd.CommandType = CommandType.StoredProcedure;
                insertCmd.CommandText = "InsertVacationPackage";

                insertCmd.Parameters.AddWithValue("@UserID", userId);
                insertCmd.Parameters.AddWithValue("@PackageName", packageName);
                insertCmd.Parameters.AddWithValue("@StartDate", startDate.Date);
                insertCmd.Parameters.AddWithValue("@EndDate", endDate.Date);
                insertCmd.Parameters.AddWithValue("@TotalCost", 0m);
                insertCmd.Parameters.AddWithValue("@Status", "Building");

                SqlParameter outputParam = new SqlParameter("@NewPackageID", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                insertCmd.Parameters.Add(outputParam);

                db.DoUpdateUsingCmdObj(insertCmd);

                packageId = Convert.ToInt32(outputParam.Value);
            }

            return packageId;
        }
    }
}
