using System.Text.Json.Serialization;

namespace EventsWebAPI.Models
{
    /// <summary>
    /// Represents a request from a partner travel site to obtain an API token.
    /// </summary>
    /// <remarks>
    /// This data transfer object (DTO) is used by partner sites to register or
    /// authenticate with the Events Web API. The <see cref="SiteName"/> is
    /// required, while the <see cref="Email"/> field is optional but recommended
    /// for communication and verification purposes.
    ///
    /// <para>Example request (JSON):</para>
    /// <code language="json">
    /// {
    ///   "SiteName": "TravelSite",
    ///   "Email": "contact@TravelSite.com"
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public class APIRequest
    {
        private string siteName;
        private string email;

        /// <summary>
        /// The name of the travel site making the API token request.
        /// </summary>
        /// <remarks>
        /// This value identifies the external application or website that
        /// wants to interact with the Events Web API. It must be unique per
        /// partner site and is used as a key in the <c>ApiClients</c> table.
        /// </remarks>
        /// <example>TravelSite</example>
        public string SiteName
            {
                get 
                { 
                    return siteName; 
                }
                set
                {
                    siteName = value;
                }
            }

        /// <summary>
        /// The contact email associated with the travel site.
        /// </summary>
        /// <remarks>
        /// This field is optional but can be used by the API administrator
        /// to communicate with the travel site for support or verification.
        /// </remarks>
        /// <example>contact@TravelSite.com</example>
        public string Email
            {
                get 
                { 
                    return email; 
                }
                set
                {
                    email = value;
                }
            }
        /// <summary>
        /// Default constructor used for JSON or XML deserialization.
        /// </summary>
        public APIRequest() { }
        /// <summary>
        /// Initializes a new API request with the specified site name and email.
        /// </summary>
        /// <param name="siteName">The name of the travel site.</param>
        /// <param name="email">The contact email address for the site.</param>
        public APIRequest(string siteName, string email)
            {
                SiteName = siteName;
                Email = email;
            }
        }
    }
