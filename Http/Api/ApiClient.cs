
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Class for the Api Client.
    /// </summary>
    public class ApiClient : IDisposable

    {
        #region Events

        /// <summary>
        ///  Event for sending API-Request.
        /// </summary>
        public event Action<RequestSendEventArgs> RequestSend;

        /// <summary>
        /// Event for API-Response Recieved.
        /// </summary>
        public event Action<ResponseRecievedEventArgs> ResponseRecieved;

        #endregion Events

        #region Fields

        /// <summary>
        /// Base Url of of the API e.g. https://www.testshop.com/api .
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The internal used System.Net.HttpClient.
        /// </summary>
        private HttpClient _client;

        /// <summary>
        /// Sets if the Client needs authentification for every request (e.g. Basic or Bearer)
        /// </summary>
        private bool _isUsingAuthentification = false;

        /// <summary>
        /// The ApiAuthentication (Bearer or Basic) when used, otherwise null
        /// </summary>
        private ApiAuthentication _authentication = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new Api Client.
        /// </summary>
        /// <param name="baseUrl">The base Url of the Client.</param>
        public ApiClient(string baseUrl)
        {
            BaseUrl = baseUrl;

            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        /// <summary>
        /// Creates a new Api Client.
        /// </summary>
        /// <param name="baseUrl">The base Url of the Client.</param>
        /// <param name="authentification">The Authentification Style.</param>
        /// <exception cref="ArgumentNullException">Authentification is null.</exception>
        public ApiClient(string baseUrl, ApiAuthentication authentification)
        {
            BaseUrl = baseUrl;

            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            if (authentification != null)
            {
                _authentication = authentification;
                _isUsingAuthentification = true;
            }
            else
            {
                throw new ArgumentNullException("Authentifcation cant be null if passed in Constructor");
            }
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Enables the Authentification of the Client.
        /// </summary>
        /// <param name="authentification"></param>
        /// <returns></returns>
        public bool EnableAuthentification(ApiAuthentication authentification)
        {
            if (authentification == null)
            {
                throw new ArgumentNullException("Authentifcation cant be null if enabling is called.");
            }

            _isUsingAuthentification = true;

            _authentication = authentification;

            return _isUsingAuthentification;
        }

        /// <summary>
        /// Disbales the Authentification.
        /// </summary>
        /// <returns></returns>
        public bool DisableAuthentification()
        {
            if (_isUsingAuthentification)
            {
                _isUsingAuthentification = false;
            }

            _authentication = null;

            return _isUsingAuthentification;
        }

        /// <summary>
        /// Executes the ApiResponse.
        /// </summary>
        /// <typeparam name="TResult">Response Type.</typeparam>
        /// <param name="request">The Request to send to the API-Server.</param>
        /// <returns>A Task containg the Response from the API-Server</returns>
        public async Task<TResult> Execute<TResult>(ApiRequest request) where TResult : ApiResponse
        {
            var requestMessage = new HttpRequestMessage();

            try
            {
                string requestUrl = string.Concat(BaseUrl, request.Endpoint);

                Regex regex = new Regex(@"\{([^}]+)\}");

                bool containsPlaceholders = regex.IsMatch(requestUrl);

                if (containsPlaceholders)
                {
                    int count = 0;

                    var finalRequestUrl = Regex.Replace(requestUrl, regex.ToString(), match =>
                    {
                        if (count < request.Parameters.Length)
                        {
                            return request.Parameters[count++].ToString();
                        }

                        throw new Exception("Parameter Array dosent match wildcard count!");
                    });

                    requestMessage.RequestUri = new Uri(finalRequestUrl);
                }
                else
                {
                    requestMessage.RequestUri = new Uri(requestUrl);
                }

                switch (request.Method)
                {
                    case ApiRequest.HttpMethod.Get:

                        requestMessage.Method = HttpMethod.Get;

                        break;

                    case ApiRequest.HttpMethod.Post:

                        requestMessage.Method = HttpMethod.Post;

                        break;

                    case ApiRequest.HttpMethod.Delete:

                        requestMessage.Method = HttpMethod.Delete;

                        break;

                    case ApiRequest.HttpMethod.Put:

                        requestMessage.Method = HttpMethod.Put;

                        break;

                    case ApiRequest.HttpMethod.Head:

                        requestMessage.Method = HttpMethod.Head;

                        break;

                    case ApiRequest.HttpMethod.Options:

                        requestMessage.Method = HttpMethod.Options;

                        break;
                }

                if (_isUsingAuthentification)
                {
                    requestMessage.Headers.Add(_authentication.AuthHeader.Key, _authentication.AuthHeader.Value);
                }

                foreach (var header in request.Headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }

                if (!string.IsNullOrEmpty(request.Body))
                {
                    if (request.RequestType == ApiRequest.ApiRequestType.Json || request.RequestType == ApiRequest.ApiRequestType.Xml || request.RequestType == ApiRequest.ApiRequestType.PlainText)
                    {
                        requestMessage.Content = new StringContent(request.Body, System.Text.Encoding.UTF8, ApiFactory.MimeTypes[request.RequestType]);
                    }
                }

                OnRequestSend(request);

                var responseMessage = await _client.SendAsync(requestMessage);

                if (responseMessage != null)
                {
                    ApiResponse.HttpStatusCodes apiResponseStatusCode = ApiResponse.HttpStatusCodes.UnkownOrUnsupportedStatus;

                    switch (responseMessage.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.Ok;

                            break;

                        case System.Net.HttpStatusCode.Created:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.Created;

                            break;

                        case System.Net.HttpStatusCode.Accepted:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.Accepted;

                            break;

                        case System.Net.HttpStatusCode.NoContent:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.NoContent;

                            break;

                        case System.Net.HttpStatusCode.BadRequest:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.BadRequest;

                            break;

                        case System.Net.HttpStatusCode.Unauthorized:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.Unauthorized;

                            break;

                        case System.Net.HttpStatusCode.Forbidden:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.Forbidden;

                            break;

                        case System.Net.HttpStatusCode.NotFound:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.NotFound;

                            break;

                        case System.Net.HttpStatusCode.MethodNotAllowed:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.MethodNotAllowed;

                            break;

                        case System.Net.HttpStatusCode.RequestTimeout:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.RequestTimeout;

                            break;

                        case System.Net.HttpStatusCode.InternalServerError:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.InternalServerError;

                            break;

                        case System.Net.HttpStatusCode.NotImplemented:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.NotImplemented;

                            break;

                        case System.Net.HttpStatusCode.BadGateway:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.BadGateway;

                            break;

                        case System.Net.HttpStatusCode.ServiceUnavailable:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.ServiceUnavailable;

                            break;

                        case System.Net.HttpStatusCode.GatewayTimeout:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.GatewayTimeout;

                            break;

                        case System.Net.HttpStatusCode.HttpVersionNotSupported:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.HttpVersionNotSupported;

                            break;

                        default:

                            apiResponseStatusCode = ApiResponse.HttpStatusCodes.UnkownOrUnsupportedStatus;

                            break;
                    }

                    var content = await responseMessage.Content.ReadAsStreamAsync();

                    var apiResponse = (TResult)Activator.CreateInstance(typeof(TResult), apiResponseStatusCode, content);

                    OnResponseRetrieved(apiResponse);

                    return apiResponse;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Disposes the Ressources.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }

        /// <summary>
        /// Releases the Event when an Request has been send.
        /// </summary>
        /// <param name="request">The request which has been send.</param>
        public void OnRequestSend(ApiRequest request)
        {
            RequestSend?.Invoke(new RequestSendEventArgs(request));
        }

        /// <summary>
        ///  Releases the Event when an Response has been retrieved.
        /// </summary>
        /// <param name="response">The retrived response.</param>
        public void OnResponseRetrieved(ApiResponse response)
        {
            ResponseRecieved?.Invoke(new ResponseRecievedEventArgs(response));
        }

        #endregion Methods
    }
}