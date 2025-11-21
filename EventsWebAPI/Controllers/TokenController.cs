using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Utilities;
using System.Data.SqlClient;
using EventsWebAPI.Models;

namespace EventsWebAPI.Controllers
{
    /// <summary>
    /// Issues API tokens for verified partner travel sites.
    /// </summary>
    /// <remarks>
    /// This controller provides an endpoint to create or retrieve a token for a partner site.
    /// It maps to the stored procedure <c>spCreateToken</c>, which returns a single-row result
    /// containing the following columns:
    /// <list type="bullet">
    /// <item><description><c>TravelSiteID</c> — The unique identifier for the partner site.</description></item>
    /// <item><description><c>SiteName</c> — The registered name of the partner site.</description></item>
    /// <item><description><c>Token</c> — The API token to be used in authenticated requests.</description></item>
    /// <item><description><c>Email</c> — Contact email on record.</description></item>
    /// <item><description><c>DateIssued</c> — The datetime when the token was issued.</description></item>
    /// </list>
    ///
    /// <para>Example request (JSON):</para>
    /// <code language="json">
    /// {
    ///   "SiteName": "TravelSiteModification",
    ///   "Email": "contact@travelsitemodification.com"
    /// }
    /// </code>
    ///
    /// <para>Example success response (JSON):</para>
    /// <code language="json">
    /// {
    ///   "TravelSiteID": "TravelSite",
    ///   "SiteName": "TravelSite",
    ///   "Token": "AbC123xYz890Token",
    ///   "Email": "contact@TravelSite.com",
    ///   "DateIssued": "2025-11-03T12:00:00"
    /// }
    /// </code>
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private DBConnect db = new DBConnect();
        /// <summary>
        /// Creates (or retrieves) an API token for a partner travel site.
        /// </summary>
        /// <param name="site">
        /// An object containing the partner site’s name and contact email.
        /// The <c>SiteName</c> field is required.
        /// </param>
        /// <returns>
        /// <para><b>200 OK</b> — Returns a JSON/XML object with <c>TravelSiteID</c>, <c>SiteName</c>, <c>Token</c>, <c>Email</c>, and <c>DateIssued</c>.</para>
        /// <para><b>400 Bad Request</b> — If <c>SiteName</c> is missing or the request body is null.</para>
        /// <para><b>500 Internal Server Error</b> — If the token could not be generated or no data was returned.</para>
        /// </returns>
        /// <remarks>
        /// Maps to stored procedure <c>TP_spCreateToken</c>. The procedure is expected to return a
        /// single row in the first table of the result set. The controller validates the input and
        /// returns standardized error messages for common failure cases.
        /// </remarks>
        /// <response code="200">Token issued successfully.</response>
        /// <response code="400">Missing or invalid request body.</response>
        /// <response code="500">Token creation failed or unexpected result shape.</response>
        [HttpPost("create")]
        public IActionResult CreateToken([FromBody] APIRequest site)
        {
            if (site == null || String.IsNullOrEmpty(site.SiteName))
            {
                return BadRequest("Site name is required.");
            }

            SqlCommand cmd = new SqlCommand("TP_spCreateToken");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SiteName", site.SiteName);
            cmd.Parameters.AddWithValue("@Email", site.Email ?? "");

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds == null)
            {
                return StatusCode(500, "Failed to generate token.");
            }
            DataRow row = ds.Tables[0].Rows[0];

            return Ok(new
            {
                TravelSiteID = row["TravelSiteID"],
                SiteName = row["SiteName"],
                Token = row["Token"],
                Email = row["Email"],
                DateIssued = row["DateIssued"]
            });
        }
    }
}
