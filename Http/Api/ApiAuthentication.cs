using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static MinimalApiClient.Http.Api.ApiAuthenticationHeader;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Manages API authentication types and headers for minimal API clients.
    /// </summary>
    public class ApiAuthentication
    {
        /// <summary>
        /// Defined Key for HTTP-Header.
        /// </summary>
        public const string AUTHORIZATION_HEADER_KEY = "Authorization";

        /// <summary>
        ///  Private access to the authentifcation header data.
        /// </summary>
        private ApiAuthenticationHeader _authorizationHeader;

        /// <summary>
        /// Holds the builded AuthorizationHeader.
        /// </summary>
        public KeyValuePair<string, string> AuthHeader { get; private set; }

        /// <summary>
        /// Creates a new Api Authentifcation Header.
        /// </summary>
        /// <param name="authorizationHeader">The autohrization Header.</param>
        public ApiAuthentication(ApiAuthenticationHeader authorizationHeader)
        {
            _authorizationHeader = authorizationHeader;

            BuildAuthHeader();
        }

        /// <summary>
        /// Builds the authorization header based on the authentication type and token/credentials.
        /// </summary>
        private void BuildAuthHeader()
        {
            // Get the format for the selected authentication type (e.g., "Bearer {0}")
            string authKeyPart = string.Join("", _authorizationHeader.AuthenticationType, "{0}");

            // Define a regular expression to find placeholders (e.g., "{0}") in the format string.
            Regex regex = new Regex(@"\{([^}]+)\}");

            // Check if the format string contains any placeholders.
            bool containsPlaceholders = regex.IsMatch(authKeyPart);

            if (containsPlaceholders)
            {
                // Replace placeholders with the token or credentials.
                var finalHeaderValue = regex.Replace(authKeyPart, match => _authorizationHeader.TokenOrCredentials);

                // Store the completed authorization header.
                AuthHeader = new KeyValuePair<string, string>(AUTHORIZATION_HEADER_KEY, finalHeaderValue);
            }
        }
    }
}