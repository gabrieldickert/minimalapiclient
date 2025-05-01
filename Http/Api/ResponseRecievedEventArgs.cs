using System;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Event arguments for a response-received event, containing the details of the received API response.
    /// </summary>
    public class ResponseRecievedEventArgs
    {
        /// <summary>
        ///  The Timestamp when the Response came in.
        /// </summary>
        public DateTime ResponsedAt { get; set; }

        /// <summary>
        /// The API response that was received.
        /// </summary>
        public ApiResponse ReceivedResponse { get; set; }

        /// <summary>
        /// Initializes a new instance of the ResponseRecievedEventArgs class with the specified API response.
        /// </summary>
        /// <param name="response">The API response to be associated with this event.</param>
        public ResponseRecievedEventArgs(ApiResponse response)
        {
            ReceivedResponse = response;

            ResponsedAt = DateTime.Now;
        }
    }
}