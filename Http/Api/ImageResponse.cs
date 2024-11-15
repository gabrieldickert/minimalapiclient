using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MinimalApiClient.Http.Api
{
    public class ImageResponse : ApiResponse
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="content"></param>
        public ImageResponse(HttpStatusCodes statusCode, Stream content) : base(statusCode, content)
        {
        }

        /// <summary>
        /// Retrieves the Image from the Result.
        /// </summary>
        /// <returns>The Image.</returns>
        public async Task<Image> GetImage()
        {
            var ms = new MemoryStream();

            await _content.CopyToAsync(ms);

            return Image.FromStream(ms);
        }
    }
}