using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TestSoft.FileStorageWPFLibrary.Contracts;
using TestSoft.FileStorageWPFLibrary.Models;

namespace TestSoft.FileStorageWPFLibrary
{
    public class FileStorageApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        // Конструктор с передачей URL API и HttpClient
        public FileStorageApiClient(HttpClient httpClient, string apiBaseUrl)
        {
            _httpClient = httpClient;
            _apiBaseUrl = apiBaseUrl;
            SetRequestHeaders("application/json"); // Заголовок по умолчанию
        }

        // Метод для установки заголовков
        private void SetRequestHeaders(string acceptHeader)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        }

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
