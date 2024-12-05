using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TestSoft.FileStorageWPFLibrary.Contracts;
using TestSoft.FileStorageWPFLibrary.Models;

namespace TestSoft.FileStorageWPFLibrary
{
    /// <summary>
    /// Client for interacting with the FileStorage API.
    /// </summary>
    public class FileStorageApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageApiClient"/> class with specified HttpClient and API base URL.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to send requests.</param>
        /// <param name="apiBaseUrl">The base URL of the FileStorage API.</param>
        public FileStorageApiClient(HttpClient httpClient, string apiBaseUrl)
        {
            _httpClient = httpClient;
            _apiBaseUrl = apiBaseUrl;
            SetRequestHeaders("application/json"); // Default header for JSON responses
        }

        /// <summary>
        /// Sets the default request headers for the HttpClient.
        /// </summary>
        /// <param name="acceptHeader">The value for the Accept header.</param>
        private void SetRequestHeaders(string acceptHeader)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        }

        /// <summary>
        /// Retrieves a JSON object from the FileStorage API by its ID.
        /// </summary>
        /// <param name="id">The ID of the JSON object.</param>
        /// <returns>Returns an <see cref="ApiResponse{T}"/> containing the JSON object or an error.</returns>
        public async Task<ApiResponse<JsonObjectDto?>> GetJsonObject(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<JsonObjectDto>(content);
                    return new ApiResponse<JsonObjectDto?>(jsonObject);
                }

                return new ApiResponse<JsonObjectDto?>(response.ReasonPhrase ?? "Unknown error", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return new ApiResponse<JsonObjectDto?>(ex.Message, 500); // 500 - internal server error
            }
        }

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
                    return new ApiResponse<JsonObjectDto?>(resultObject);
                }

                return new ApiResponse<JsonObjectDto?>(response.ReasonPhrase ?? "Unknown error", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return new ApiResponse<JsonObjectDto?>(ex.Message, 500);
            }
        }

        /// <summary>
        /// Deletes a JSON object from the FileStorage API by its ID.
        /// </summary>
        /// <param name="id">The ID of the JSON object to delete.</param>
        /// <returns>Returns an <see cref="ApiResponse{T}"/> indicating the success or failure of the deletion.</returns>
        public async Task<ApiResponse<bool>> DeleteJsonObject(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>(true);
                }

                return new ApiResponse<bool>(response.ReasonPhrase ?? "Unknown error", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(ex.Message, 500);
            }
        }

        /// <summary>
        /// Applies a patch to a JSON object in the FileStorage API.
        /// </summary>
        /// <param name="id">The ID of the JSON object to patch.</param>
        /// <param name="patchRequest">The patch request containing the operations to apply.</param>
        /// <returns>Returns an <see cref="ApiResponse{T}"/> containing the patched JSON object or an error.</returns>
        public async Task<ApiResponse<JsonObjectDto?>> ApplyPatch(Guid id, JsonPatchRequestDto patchRequest)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(patchRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"{_apiBaseUrl}/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    var resultContent = await response.Content.ReadAsStringAsync();
                    var resultObject = JsonConvert.DeserializeObject<JsonObjectDto>(resultContent);
                    return new ApiResponse<JsonObjectDto?>(resultObject);
                }

                return new ApiResponse<JsonObjectDto?>(response.ReasonPhrase ?? "Unknown error", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return new ApiResponse<JsonObjectDto?>(ex.Message, 500);
            }
        }
    }
}
