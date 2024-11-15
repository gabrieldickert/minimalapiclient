
using System.Collections.Generic;
using static MinimalApiClient.Http.Api.ApiRequest;

namespace MinimalApiClient.Http.Api
{
    public static class ApiFactory
    {
        public static Dictionary<ApiRequestType, string> MimeTypes = new Dictionary<ApiRequestType, string>()
        {
            { ApiRequest.ApiRequestType.PlainText,"text/plain" },
            { ApiRequest.ApiRequestType.HTML,"text/html" },
            { ApiRequest.ApiRequestType.Json,"application/json" },
            { ApiRequest.ApiRequestType.Xml,"application/xml" },
            { ApiRequest.ApiRequestType.Binary,"application/octet-stream" },
            { ApiRequest.ApiRequestType.FormData,"application/x-www-form-urlencoded" },
            { ApiRequest.ApiRequestType.MultiPartFormData,"multipart/form-data" },
            { ApiRequest.ApiRequestType.ImageJpeg,"image/jpeg" },
            { ApiRequest.ApiRequestType.ImagePng,"image/png" }
        };

        public static ApiClient BuildClient(string baseUrl)
        {
            return new ApiClient(baseUrl);
        }
    }
}