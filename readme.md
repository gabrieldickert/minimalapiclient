# MinimalApiClient

A simple, flexible API client built for small projects using `C#`. This client allows you to make HTTP requests with various HTTP methods and supports Basic and Bearer token authentication. It can handle various request and response types, including JSON, Binary, and Images, making it easy to integrate with RESTful APIs.

## Note

This Project should only be used in personal projects. I Do not recommend in any circumstances to use this in a production environment at all.

## Features

* **Supports common HTTP methods**: GET, POST, PUT, PATCH, DELETE, OPTIONS, HEAD.

* **Flexible request and response types**: PlainText, Json, Xml, FormData, MultiPartFormData, Binary, ImagePng, ImageJpeg, HTML.

* **Simple authentication setup**: Supports Basic and Bearer tokens.

* **Event handling** for sending requests and receiving responses.

* **JSON deserialization** of responses to specified types.

* **Image retrieval** from binary responses.

* **Dynamic URI parameters** using wildcard markers.

## Installation

To use this API client in your project, add it as a reference and include the necessary namespaces:

```csharp
using MinimalApiClient.Http.Api;
using Newtonsoft.Json.Linq; // For JSON handling
using System.Drawing; // Optional, for ImageResponse
```csharp
using MinimalApiClient.Http.Api;  
using Newtonsoft.Json.Linq;
#optional
using System.Drawing;
```
## Usage

### 1. Create an ApiClient instance

Initialize the `ApiClient` with the base URL of your API:
```csharp
ApiClient client = new ApiClient("https://jsonplaceholder.typicode.com/");
```

You can also initialize with authentication:
```csharp
// Example with Basic Authentication
var basicAuthHeader = new ApiAuthenticationHeader(ApiAuthenticationHeader.PredefinedAuthenticationTypes.Basic, "your_username:your_password");
ApiClient clientWithBasicAuth = new ApiClient("[https://api.example.com/](https://api.example.com/)", new ApiAuthentication(basicAuthHeader));

// Example with Bearer Authentication (token obtained separately)
var bearerAuthHeader = new ApiAuthenticationHeader(ApiAuthenticationHeader.PredefinedAuthenticationTypes.Bearer, "your_access_token");
ApiClient clientWithBearerAuth = new ApiClient("[https://api.example.com/](https://api.example.com/)", new ApiAuthentication(bearerAuthHeader));
```



### 2. Example - Obtain and Use a Bearer Token
```csharp
var bearerTokenRequest = new ApiRequest("/oauth/token", ApiRequest.HttpMethod.Post, ApiRequest.ApiRequestType.Json);
```
Set up the body with required authentication fields:
```csharp
JObject body = new JObject  
{  
    ["client_id"] = "administration",  
    ["grant_type"] = "password",  
    ["scopes"] = "write",  
    ["username"] = "user",  
    ["password"] = "test123456"  
};
```
Assign the body to the request:
```csharp
bearerTokenRequest.SetBody(body.ToString());
```
Send the request to obtain the bearer token (using await for asynchronous operation):
```csharp
JsonApiResponse tokenResponse = await client.Execute<JsonApiResponse>(bearerTokenRequest);
JObject bearerTokenJson = await tokenResponse.GetJsonFromContentAsync<JObject>();
```
Extract the access token and enable authentication on the client:
```csharp
var accessToken = bearerTokenJson["access_token"].Value<string>();
client.EnableAuthentification(new ApiAuthentication(ApiAuthenticationHeader.PredefinedAuthenticationTypes.Bearer, accessToken));
```
### 3. Basic CRUD Operations

Once authenticated (if required), you can proceed with common CRUD operations:

#### GET Request

Retrieve a specific resource using GET:
```csharp
// Endpoint with a wildcard {0}
var reqGet = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Get, ApiRequest.ApiRequestType.Json);
// Set the parameter value (replaces {0})
reqGet.SetUriParameters(new object[] { 35 });
JsonApiResponse resultGetResponse = await client.Execute<JsonApiResponse>(reqGet);
JObject resultGet = await resultGetResponse.GetJsonFromContentAsync<JObject>();
Console.WriteLine($"GET Result:\n{resultGet.ToString(Newtonsoft.Json.Formatting.Indented)}");
```
#### POST Request

Create a new resource using POST:
```csharp
var reqPost = new ApiRequest("todos/", ApiRequest.HttpMethod.Post, ApiRequest.ApiRequestType.Json);

var reqPostBody = new JObject()
{
    ["title"] = "minimalApi",
    ["body"] = "MinimalApiTest",
    ["userId"] = 1 // Example user ID
};

// Set the JSON body
reqPost.SetBody(reqPostBody);

JsonApiResponse resultPostResponse = await client.Execute<JsonApiResponse>(reqPost);
JObject resultPost = await resultPostResponse.GetJsonFromContentAsync<JObject>();

Console.WriteLine($"POST Result:\n{resultPost}");
```
#### PUT Request

Update a resource with PUT:
```csharp
var reqPut = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Put, ApiRequest.ApiRequestType.Json);

var reqPutBody = new JObject()
{
    ["title"] = "newerTitle",
    ["body"] = "Updated the Text of the post.",
    ["userId"] = resultPost["userId"], // Use ID from POST result
    ["id"] = resultPost["id"] // Use ID from POST result
};

// Set the URI parameter for the specific resource ID
reqPut.SetUriParameters(new object[] { resultPost["id"] });
// Set the updated JSON body
reqPut.SetBody(reqPutBody);

// Note: jsonplaceholder might return an array for PUT, adjust deserialization if needed
JsonApiResponse resultPutResponse = await client.Execute<JsonApiResponse>(reqPut);
JObject resultPut = await resultPutResponse.GetJsonFromContentAsync<JObject>(); // Assuming it returns an object

Console.WriteLine($"PUT Result:\n{resultPut}");
```
#### PATCH Request

Partially update a resource with PATCH:
```csharp
var reqPatch = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Patch, ApiRequest.ApiRequestType.Json);

var reqPatchBody = new JObject() { ["title"] = "patchedTitle" };

// Set the URI parameter for the specific resource ID
reqPatch.SetUriParameters(new object[] { resultPut["id"] }); // Use ID from PUT result
// Set the partial JSON body
reqPatch.SetBody(reqPatchBody);

JsonApiResponse resultPatchResponse = await client.Execute<JsonApiResponse>(reqPatch);
JObject resultPatch = await resultPatchResponse.GetJsonFromContentAsync<JObject>();

Console.WriteLine($"PATCH Result:\n{resultPatch}");
```
#### DELETE Request

Delete a specific resource using DELETE:
```csharp
var reqDelete = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Delete, ApiRequest.ApiRequestType.Json);

// Set the URI parameter for the specific resource ID
reqDelete.SetUriParameters(new object[] { resultPatch["id"] }); // Use ID from PATCH result

ApiResponse resultDeleteResponse = await client.Execute<ApiResponse>(reqDelete); // DELETE might return no content

// Check status code or content if expected
Console.WriteLine($"DELETE Status Code: {resultDeleteResponse.StatusCode}");
```
#### Image Request

You can retrieve an Image from an Api as following:
```csharp
var imageReq = new ApiRequest("/id/{0}/2758/3622", ApiRequest.HttpMethod.Get, ApiRequest.ApiRequestType.Binary);
imageReq.SetUriParameters(new object[] { 35 });
var imageResult = await client.Execute<ImageResponse>(imageReq).Result.GetImage();
imageResult.Save("result.png");
```
### 4. Adding Custom Headers
You can add custom headers to any request:
```csharp
var requestWithHeader = new ApiRequest("some/endpoint", ApiRequest.HttpMethod.Get, ApiRequest.ApiRequestType.Json);
requestWithHeader.AddRequestHeader("X-Custom-Header", "SomeValue");
```
## Class Overview

### ApiClient

The main class for sending HTTP requests.

* Manages the base URL and the internal `System.Net.HttpClient`.

* Handles authentication if enabled.

* Provides the `Execute<TResult>(ApiRequest request)` method to send requests asynchronously.

* Exposes `RequestSend` and `ResponseRecieved` events.

* Implements `IDisposable` to properly dispose the internal `HttpClient`.

### ApiRequest

Represents an individual API request.

* Defines the `HttpMethod` enum (Get, Post, Put, Patch, Delete, Options, Head).

* Defines the `ApiRequestType` enum (PlainText, Json, Xml, FormData, MultiPartFormData, Binary, ImagePng, ImageJpeg, HTML).

* Uses `WILDCARD_START = "{"` and `WILDCARD_END = "}"` for dynamic URI parameters.

* Properties for `Endpoint`, `Method`, `RequestType`, `Headers`, `Body`, and `Parameters`.

* Methods to `AddRequestHeader`, `SetUriParameters`, and `SetBody` (for `JObject` or string).

* `SetBody` throws an `ArgumentException` if used with HTTP methods that do not support a body (like GET).

### ApiResponse, JsonApiResponse & ImageResponse

Base and derived classes for handling API responses.

* **`ApiResponse`**: The base class for all responses.

  * Contains the `HttpStatusCodes` enum with standard HTTP status codes.

  * Stores the `StatusCode` and the response content as a `Stream`.

  * Provides `GetContentAsStream<T>()` to get the content as a specified stream type.

  * Provides `GetContentAsStringAsync()` to read the content as a UTF-8 string.

  * Throws `InvalidOperationException` if the content stream is not initialized when reading.

* **`JsonApiResponse`**: Inherits from `ApiResponse` for JSON responses.

  * Provides `GetJsonFromContentAsync<T>()` to deserialize the JSON content to an object of type `T`.

  * Throws an `Exception` if deserialization fails, wrapping the inner exception.

* **`ImageResponse`**: Inherits from `ApiResponse` for image responses.

  * Provides `GetImage()` to retrieve the image content as a `System.Drawing.Image` object.

### ApiAuthentication

Manages API authentication details.

* Contains the `AUTHORIZATION_HEADER_KEY = "Authorization"` constant.

* Holds an `ApiAuthenticationHeader` instance.

* Builds the final authorization header string (`AuthHeader`) based on the authentication type and token/credentials.

### ApiAuthenticationHeader

Represents the data needed to build an authentication header.

* Defines the `PredefinedAuthenticationTypes` enum (Basic, Bearer).

* Stores the `AuthenticationType` (string) and `TokenOrCredentials` (string).

* Has constructors to initialize with a string type or a `PredefinedAuthenticationTypes` enum value.

### Events

* `RequestSendEventArgs`: Used for the `ApiClient.RequestSend` event. Contains the `SentRequest` (`ApiRequest`) and the `RequestedAt` timestamp.

* `ResponseRecievedEventArgs`: Used for the `ApiClient.ResponseRecieved` event. Contains the `ReceivedResponse` (`ApiResponse`) and the `ResponsedAt` timestamp.
