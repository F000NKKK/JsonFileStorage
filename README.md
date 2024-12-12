# TestSoft File Storage Manager — Data Management via REST API
TestSoft File Storage Manager is a powerful application that allows you to interact with a database through a convenient interface. With this tool, you can add, delete, update, and read JSON objects, as well as perform search operations by ID.

## Key Features:
- Create, update, delete, and read JSON objects.
- Support for PATCH requests to modify individual object fields.
- Error handling with detailed error messages.
- Scalable and adaptable API for various tasks.

## Commands and Actions:
- **Create**  
  Creates a new JSON object in the database.

- **Read**  
  Reads a JSON object by the specified ID.

- **Update**  
  Updates a JSON object using a PATCH request (modify individual fields).

- **Delete**  
  Deletes a JSON object by the specified ID.

## Usage:
1. Launch the application and connect to the API.
2. Use the corresponding buttons in the interface to perform the desired operations (create, read, update, or delete objects).
3. Upon successful completion of an operation, a corresponding message will be displayed, or in case of an error, the issue will be described.

## Example API Requests:

- **POST /api/json**  
  Creates a new JSON object in the database.

- **GET /api/json/{id}**  
  Retrieves a JSON object by the specified ID.

- **PATCH /api/json/{id}**  
  Updates a JSON object with the specified ID.

- **DELETE /api/json/{id}**  
  Deletes the JSON object with the specified ID.

## Example Usage in Client:

```csharp
/// <summary>
/// Creates a new JSON object in the FileStorage API.
/// </summary>
/// <param name="jsonObject">The JSON object to create.</param>
/// <returns>Returns an <see cref="ApiResponse{T}"/> containing the created JSON object or an error.</returns>
public async Task<ApiResponse<JsonObjectDto?>> CreateJsonObject(JsonObjectDto jsonObject)
{
    try
    {
        var content = new StringContent(JsonConvert.SerializeObject(jsonObject), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_apiBaseUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var resultContent = await response.Content.ReadAsStringAsync();

            var resultObject = JsonConvert.DeserializeObject<JsonObjectDto>(resultContent);

            if (response.Headers.Location != null)
            {
                var id = response.Headers.Location.AbsolutePath.Split('/').Last();
                resultObject.Id = Guid.Parse(id);
            }

            return new ApiResponse<JsonObjectDto?>(resultObject);
        }

        return new ApiResponse<JsonObjectDto?>(response.ReasonPhrase ?? "Unknown error", (int)response.StatusCode);
    }
    catch (Exception ex)
    {
        return new ApiResponse<JsonObjectDto?>(ex.Message, 500);
    }
}
```
## Download the assembled project

| Releases v3.x.x | Download link                                                 |.NET version    |
|:-------------:|:---------------------------------------------------------------:|:--------------:|
| v3.1.2 Build | [Win11 x64 Release](https://github.com/F000NKKK/JsonFileStorage/releases/download/v3.1.2/JsonFileStorage_Win11_x64_build_v3.1.2.zip) |SDK .NET v9.0|
| v3.0.0 Build | [Win11 x64 Release](https://github.com/F000NKKK/JsonFileStorage/releases/download/v3.0.0/JsonFileStorage_Win11_x64_build_v3.0.0.zip) |SDK .NET v9.0|

| Releases v2.x.x | Download link                                                 |.NET version    |
|:-------------:|:---------------------------------------------------------------:|:--------------:|
| v2.0.0 Build | [Win11 x64 Release](https://github.com/F000NKKK/JsonFileStorage/releases/download/v2.0.0/JsonFileStorage_Win11_x64_build_v2.0.0.zip) |SDK .NET v8.0|

| Releases v1.x.x | Download link                                                 |.NET version    |
|:-------------:|:---------------------------------------------------------------:|:--------------:|
| v1.5.0 Build | [Win11 x64 Release](https://github.com/F000NKKK/JsonFileStorage/releases/download/v1.5.0/JsonFileStorage_Win11_x64_build_v1.5.0.zip) |SDK .NET v8.0|
