using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Represents an API request, including HTTP method, request type, headers, and body.
    /// </summary>
    public class ApiRequest
    {
        /// <summary>
        /// Supported HTTP methods for API requests.
        /// </summary>
        public enum HttpMethod
        {
            Get = 0,
            Post = 1,
            Put = 2,
            Patch = 3,
            Delete = 4,
            Options = 5,
            Head = 6
        }

        /// <summary>
        /// Defines types of API requests, such as JSON, XML, and binary data.
        /// </summary>
        public enum ApiRequestType
        {
            PlainText = 0,
            Json = 1,
            Xml = 2,
            FormData = 3,
            MultiPartFormData = 4,
            Binary = 5,
            ImagePng = 6,
            ImageJpeg = 7,
            HTML = 8
        }

        /// <summary>
        /// Markers for wildcard parameters in URI paths.
        /// </summary>
        public const string WILDCARD_START = "{";
        public const string WILDCARD_END = "}";

        /// <summary>
        /// API endpoint URI for the request.
        /// </summary>
        public string Endpoint { get; private set; }

        /// <summary>
        /// The HTTP method to be used for the request.
        /// </summary>
        public HttpMethod Method { get; private set; }

        /// <summary>
        /// The type of content the request is expected to handle.
        /// </summary>
        public ApiRequestType RequestType { get; private set; }

        /// <summary>
        /// Dictionary to store headers for the request.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Stores the request body, which can be a JSON string or plain text.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Array to hold URI parameters for dynamic endpoints.
        /// </summary>
        public object[] Parameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ApiRequest class with specified endpoint, method, and request type.
        /// </summary>
        /// <param name="endpoint">API endpoint for the request.</param>
        /// <param name="method">HTTP method for the request (GET, POST, etc.).</param>
        /// <param name="requestType">The content type for the request.</param>
        public ApiRequest(string endpoint, HttpMethod method, ApiRequestType requestType)
        {
            Endpoint = endpoint;
            Method = method;
            RequestType = requestType;
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds a new header to the request.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public void AddRequestHeader(string name, string value)
        {
            Headers.Add(name, value);
        }

        /// <summary>
        /// Sets the URI parameters for the endpoint.
        /// </summary>
        /// <param name="parameters">Array of parameters to replace wildcards in the endpoint URI.</param>
        public void SetUriParameters(object[] parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Sets the request body from a JSON object. Only allowed for POST, PUT, and PATCH methods.
        /// </summary>
        /// <param name="body">JSON object to be converted to a string and set as the request body.</param>
        /// <exception cref="ArgumentException">Thrown if the HTTP method does not support a request body.</exception>
        public void SetBody(JObject body)
        {
            // Validate that the HTTP method allows a request body.
            if (Method != HttpMethod.Post && Method != HttpMethod.Put && Method != HttpMethod.Patch)
            {
                throw new ArgumentException("Setting Body for Request with Method: " + Method.ToString() + " is not supported");
            }

            // Convert the JSON object to a string and set as the request body.
            Body = body.ToString();
        }

        /// <summary>
        /// Sets the request body from a plain text string. Only allowed for POST, PUT, and PATCH methods.
        /// </summary>
        /// <param name="body">The body content as a plain text string.</param>
        /// <exception cref="ArgumentException">Thrown if the HTTP method does not support a request body.</exception>
        public void SetBody(string body)
        {
            // Validate that the HTTP method allows a request body.
            if (Method != HttpMethod.Post && Method != HttpMethod.Put && Method != HttpMethod.Patch)
            {
                throw new ArgumentException("Setting Body for Request with Method: " + Method.ToString() + " is not supported");
            }

            // Set the plain text as the request body.
            Body = body;
        }
    }
}
