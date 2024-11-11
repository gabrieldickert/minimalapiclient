using Newtonsoft.Json;
using System.Text;

namespace MinimalApiClient.Http.Api
{
    /// <summary>
    /// Represents an API response with JSON content, extending the base ApiResponse class.
    /// Provides a method to deserialize JSON content to a specified type.
    /// </summary>
    public class JsonApiResponse : ApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the JsonApiResponse class with status code and content.
        /// </summary>
        /// <param name="statusCode">HTTP status code of the response.</param>
        /// <param name="content">Stream containing the JSON content of the response.</param>
        public JsonApiResponse(HttpStatusCodes statusCode, Stream content) : base(statusCode, content)
        {
        }

        /// <summary>
        /// Asynchronously reads JSON content from the response stream and deserializes it to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type to which the JSON content should be deserialized.</typeparam>
        /// <returns>An instance of type T representing the deserialized JSON content.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the content stream is not initialized.</exception>
        /// <exception cref="Exception">Thrown if the content cannot be deserialized to the specified type.</exception>
        public async Task<T> GetJsonFromContentAsync<T>() where T : class
        {
            // Check if the content stream is initialized before attempting to read it.
            if (_content == null)
            {
                throw new InvalidOperationException("Content stream is not initialized.");
            }

            // Use `using` blocks to ensure proper disposal of streams after reading.
            await using (_content)
            {
                await using (var memoryStream = new MemoryStream())
                {
                    // Copy the content to a memory stream to work with it as a byte array.
                    await _content.CopyToAsync(memoryStream);
                    byte[] data = memoryStream.ToArray();

                    // Convert the byte array to a UTF-8 JSON string.
                    string jsonString = Encoding.UTF8.GetString(data);

                    try
                    {
                        // Deserialize the JSON string to the specified type T and return it.
                        return JsonConvert.DeserializeObject<T>(jsonString);
                    }
                    catch (Exception ex)
                    {
                        // Wrap and throw an exception if deserialization fails, with information about the target type.
                        throw new Exception("Could not deserialize Response Content to object of type: " + typeof(T).ToString()+""+jsonString, ex.InnerException);
                    }
                }
            }
        }
    }
}