using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalApiClient.Http.Api
{
    public class ApiAuthenticationHeader
    {
        /// <summary>
        /// Defines supported authentication types: Basic and Bearer for now.
        /// </summary>
        public enum PredefinedAuthenticationTypes
        {
            Basic,
            Bearer,
        }

        public string AuthenticationType { get; private set; }

        public string TokenOrCredentials { get; private set; }

        public ApiAuthenticationHeader(string authenticationType, string tokenOrCredentials)
        {
            AuthenticationType = authenticationType;

            TokenOrCredentials = tokenOrCredentials;
        }

        public ApiAuthenticationHeader(PredefinedAuthenticationTypes predefinedAuthType, string tokenOrCredentials)
        {
            AuthenticationType = predefinedAuthType.ToString();

            TokenOrCredentials = tokenOrCredentials;
        }
    }
}