using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Manages API authentication types and headers for minimal API clients.
    /// </summary>
    public class ApiAuthentication
    {
        /// <summary>
        /// Defines supported authentication types: Basic and Bearer.
        /// </summary>
        public enum AuthenticationTypes
        {
            Basic,
            Bearer,
        }

        /// <summary>
        /// Maps authentication types to their respective header formats.
        /// </summary>
        private static readonly Dictionary<AuthenticationTypes, string> AuthenticationHeaders = new Dictionary<AuthenticationTypes, string>()
        {
            { AuthenticationTypes.Basic, "Basic {0}" },
            { AuthenticationTypes.Bearer, "Bearer {0}" },
        };

        /// <summary>
        /// The selected authentication type (Basic or Bearer).
        /// </summary>
        public AuthenticationTypes AuthenticationType { get; set; }

        /// <summary>
        /// Stores the provided token or credentials used in the authentication header.
        /// </summary>
        public string TokenOrCredentials { get; private set; }

        /// <summary>
        /// Stores the generated authorization header as a key-value pair.
        /// </summary>
        public KeyValuePair<string, string> AuthHeader { get; private set; }

        /// <summary>
        /// Constructor that initializes the ApiAuthentication with specified type and token/credentials.
        /// </summary>
        /// <param name="authenticationType">Type of authentication (Basic or Bearer).</param>
        /// <param name="tokenOrCredentials">Token or credentials for the authentication header.</param>
        public ApiAuthentication(AuthenticationTypes authenticationType, string tokenOrCredentials)
        {
            AuthenticationType = authenticationType;
            TokenOrCredentials = tokenOrCredentials;
            BuildAuthHeader();
        }

        /// <summary>
        /// Builds the authorization header based on the authentication type and token/credentials.
        /// </summary>
        private void BuildAuthHeader()
        {
            // Get the format for the selected authentication type (e.g., "Bearer {0}")
            string authKeyPart = AuthenticationHeaders[AuthenticationType];

            // Define a regular expression to find placeholders (e.g., "{0}") in the format string.
            Regex regex = new Regex(@"\{([^}]+)\}");

            // Check if the format string contains any placeholders.
            bool containsPlaceholders = regex.IsMatch(authKeyPart);

            if (containsPlaceholders)
            {
                // Replace placeholders with the token or credentials.
                var finalHeader = regex.Replace(authKeyPart, match => TokenOrCredentials);

                // Store the completed authorization header.
                AuthHeader = new KeyValuePair<string, string>("Authorization", finalHeader);
            }
        }
    }
}
