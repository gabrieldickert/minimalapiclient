using System.Text;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Represents an HTTP response, including the status code and response content.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Enum for standard HTTP status codes, covering common response statuses.
        /// </summary>
        public enum HttpStatusCodes
        {
            Ok = 200,
            Created = 201,
            Accepted = 202,
            NoContent = 204,
            BadRequest = 400,
            Unauthorized = 401,
            Forbidden = 403,
            NotFound = 404,
            MethodNotAllowed = 405,
            RequestTimeout = 408,
            TooManyRequests = 429,
            InternalServerError = 500,
            NotImplemented = 501,
            BadGateway = 502,
            ServiceUnavailable = 503,
            GatewayTimeout = 504,
            HttpVersionNotSupported = 505,
            UnkownOrUnsupportedStatus = 999
        }

        /// <summary>
        /// Stores the HTTP status code of the response.
        /// </summary>
        public HttpStatusCodes StatusCode { get; private set; }

        /// <summary>
        /// A stream that holds the content of the response. Can be read as needed.
        /// </summary>
        protected Stream _content;

        /// <summary>
        /// Default constructor for an empty ApiResponse.
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Constructor that initializes the ApiResponse with a specific status code and content stream.
        /// </summary>
        /// <param name="statusCode">HTTP status code of the response.</param>
        /// <param name="content">Stream representing the response content.</param>
        public ApiResponse(HttpStatusCodes statusCode, Stream content)
        {
            StatusCode = statusCode;
            _content = content;
        }

        /// <summary>
        /// Asynchronously reads the response content stream and returns it as a string.
        /// </summary>
        /// <returns>Content of the response as a UTF-8 string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the content stream is not initialized.</exception>
        public async Task<string> GetContentAsStringAsync()
        {
            // Check if the content stream is initialized before attempting to read it.
            if (_content == null)
            {
                throw new InvalidOperationException("Content stream is not initialized.");
            }

            // Use a `using` block to ensure the stream is properly disposed after reading.
            await using (_content)
            {
                // Copy content to a memory stream to enable encoding and return as a string.
                await using (var memoryStream = new MemoryStream())
                {
                    await _content.CopyToAsync(memoryStream);
                    byte[] data = memoryStream.ToArray();

                    // Convert the byte array to a UTF-8 string and return it.
                    return Encoding.UTF8.GetString(data);
                }
            }
        }
    }
}