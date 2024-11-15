# MinimalApiClient

A simple, flexible API client built for small projects using `C#`. This client allows you to make HTTP requests with various HTTP methods (GET, POST, PUT, PATCH, DELETE) and supports Basic and Bearer token authentication. It can handle JSON requests and responses, making it easy to integrate with RESTful APIs.

## Note
This Project should only be used in personal project. I Do not recommand in any circumstances to use this in a production enviroment at all.
## Features

- **Supports all common HTTP methods**: GET, POST, PUT, PATCH, DELETE
- **Flexible request types**: Json, Binary, FormData, and more
- **Simple authentication setup**: Supports Basic and Bearer tokens
- **Event handling** for sending requests and receiving responses
- **JSON deserialization** of responses for easy data handling

## Installation

To use this API client in your project, add it as a reference and include the necessary namespaces:
```csharp
using MinimalApiClient.Http.Api;  
using Newtonsoft.Json.Linq;
```
## Usage

### 1. Create an ApiClient instance

Initialize the `ApiClient` with the base URL of your API:
```csharp
ApiClient client = new ApiClient("https://jsonplaceholder.typicode.com/");
```

### 2. Example - Define a Bearer Token Request
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
Send the request to obtain the bearer token:
```csharp
var bearerTokenJson = await client.Execute<JsonApiResponse>(bearerTokenRequest).Result.GetJsonFromContentAsync<JObject>();
```
Extract and set the access token:
```csharp
var accessToken = bearerTokenJson["access_token"].Value<string>();  
client.EnableAuthentification(new ApiAuthentication(ApiAuthentication.AuthenticationTypes.Bearer, accessToken));
```
### 3. Basic CRUD Operations

Once authenticated, you can proceed with common CRUD operations:

#### GET Request

Retrieve a specific resource using GET:
```csharp
var reqGet = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Get, ApiRequest.ApiRequestType.Json);  
reqGet.SetUriParameters(new object[] { 35 });  
var resultGet = await client.Execute<JsonApiResponse>(reqGet).Result.GetJsonFromContentAsync<JObject>();  
Console.WriteLine($"GET Result:\n{resultGet.ToString(Newtonsoft.Json.Formatting.Indented)}");
```
#### POST Request

Create a new resource using POST:
```csharp
var reqPost = new ApiRequest("todos/", ApiRequest.HttpMethod.Post, ApiRequest.ApiRequestType.Json);  
var reqPostBody = new JObject() { ["title"] = "minimalApi", ["body"] = "MinimalApiTest", ["userId"] = resultGet["userId"] };  
reqPost.SetBody(reqPostBody);  
var resultPost = await client.Execute<JsonApiResponse>(reqPost).Result.GetJsonFromContentAsync<JObject>();  
Console.WriteLine($"POST Result:\n{resultPost}");
```
#### PUT Request

Update a resource with PUT:
```csharp
var reqPut = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Put, ApiRequest.ApiRequestType.Json);  
var reqPutBody = new JObject() { ["title"] = "newerTitle", ["body"] = "Updated the Text of the post.", ["userId"] = resultPost["userId"], ["id"] = resultPost["id"] };  
reqPut.SetUriParameters(new object[] { resultPost["id"] });  
reqPut.SetBody(reqPutBody);  
var resultPut = await client.Execute<JsonApiResponse>(reqPut).Result.GetJsonFromContentAsync<JArray>();  
Console.WriteLine($"PUT Result:\n{resultPut}");
```
#### PATCH Request

Partially update a resource with PATCH:
```csharp
var reqPatch = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Patch, ApiRequest.ApiRequestType.Json);  
var reqPatchBody = new JObject() { ["title"] = "patchedTitle" };  
reqPatch.SetUriParameters(new object[] { resultPut["id"] });  
reqPatch.SetBody(reqPatchBody);  
var resultPatch = await client.Execute<JsonApiResponse>(reqPatch).Result.GetJsonFromContentAsync<JObject>();  
Console.WriteLine($"PATCH Result:\n{resultPatch}");
```
#### DELETE Request

Delete a specific resource using DELETE:
```csharp
var reqDelete = new ApiRequest("todos/{0}", ApiRequest.HttpMethod.Delete, ApiRequest.ApiRequestType.Json);  
reqDelete.SetUriParameters(new object[] { resultPatch["id"] });  
var resultDelete = await client.Execute<JsonApiResponse>(reqDelete).Result.GetJsonFromContentAsync<JObject>();  
Console.WriteLine(resultDelete);
```
## Class Overview

### ApiRequest
Handles HTTP requests and includes methods to set headers, body content, and URI parameters. Supports different request types and HTTP methods.

### ApiResponse & JsonApiResponse
Represents responses, with `JsonApiResponse` specifically providing JSON deserialization to the specified data type.

### ApiAuthentication
Manages API authentication and currently supports Basic and Bearer token authentication types.

### Events
- `RequestSendEventArgs` is used to handle request-sending events.
- `ResponseRecievedEventArgs` is used to handle response-receiving events.

## Error Handling

The client includes basic error handling, such as:
- **Unsupported Methods**: Throws an `ArgumentException` if body content is set for methods that don’t support it (e.g., GET).
- **Deserialization Errors**: Wraps deserialization errors in an exception for easier debugging.

