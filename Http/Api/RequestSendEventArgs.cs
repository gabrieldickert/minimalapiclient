using System;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Event arguments for a request-sending event, encapsulating the API request details.
    /// </summary>
    public class RequestSendEventArgs : EventArgs
    {
        /// <summary>
        /// The API request that was sent.
        /// </summary>
        public ApiRequest SentRequest { get; set; }

        /// <summary>
        /// Initializes a new instance of the RequestSendEventArgs class with the specified API request.
        /// </summary>
        /// <param name="req">The API request to be associated with this event.</param>
        public RequestSendEventArgs(ApiRequest req)
        {
            SentRequest = req;
        }
    }
}
