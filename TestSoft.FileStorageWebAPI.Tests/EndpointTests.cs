using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using TestSoft.FileStorageWebAPI.Contracts;

namespace TestSoft.FileStorageWebAPI.API.Tests
{
    public class EndpointTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private string _guid;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    });
                });

            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5000/api/json");
            _guid = "860a51b2-0517-4244-8283-277fe94be0e0";
        }

        [Test]
        public async Task Test_CreateJsonObject_ShouldReturnSuccess()
        {
            var newJsonObject = new JsonObjectDto
            {
                Data = new Dictionary<string, object>
                {
                    { "key1", "value1" },
                    { "key2", 123 },
                    { "key3", true }
                }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(newJsonObject),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync("/api/json", content);

            Assert.That(response.IsSuccessStatusCode, Is.True, "The 'json' API should accept POST requests and create an object.");

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<JsonObjectDto>(responseContent);

            Assert.That(createdObject, Is.Not.Null, "The response should contain the created JSON object.");
            Assert.That(createdObject?.Data?.Count, Is.EqualTo(3), "The created object should have 3 key-value pairs.");
        }

        [Test]
        public async Task Test_ReadJsonObject_ShouldReturnSuccess()
        {
            var response = await _client.GetAsync($"/api/json/{_guid}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<JsonObjectDto>(responseContent);

            Assert.That(createdObject, Is.Not.Null, "The response should contain the created JSON object.");
            Assert.That(response.IsSuccessStatusCode, Is.True, "The 'json' API should return the object successfully.");
        }

        [Test]
        public async Task Test_DeleteJsonObject_ShouldReturnSuccess()
        {
            var newJsonObject = new JsonObjectDto
            {
                Data = new Dictionary<string, object>
                {
                    { "key1", "value1" },
                    { "key2", 123 }
                }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(newJsonObject),
                Encoding.UTF8,
                "application/json"
            );

            var createResponse = await _client.PostAsync("/api/json/", content);

            var id = createResponse.Headers.ToString().Split("/json/")[1];

            Assert.That(id, Is.Not.Null.Or.Empty, "The object should be created successfully.");

            var deleteResponse = await _client.DeleteAsync($"/api/json/{id}");

            var responseDeleteContent = await deleteResponse.Content.ReadAsStringAsync();
            var deleteObject = JsonConvert.DeserializeObject<JsonObjectDto>(responseDeleteContent);

            Assert.That(deleteResponse.IsSuccessStatusCode, Is.True, "The 'json' API should accept DELETE requests and remove an object.");
        }

        [Test]
        public async Task Test_UpdateJsonObject_ShouldReturnSuccess()
        {
            var patchRequest = new JsonPatchRequestDto
            {
                Operations = new List<JsonPatchOperationDto>
                {
                    new JsonPatchOperationDto
                    {
                        Op = "replace",
                        Path = "firstName",
                        Value = "Petr"
                    }
                }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(patchRequest),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PatchAsync($"/api/json/{_guid}", content);

            Assert.That(response.IsSuccessStatusCode, Is.True, "The 'json' API should accept PATCH requests and update an object.");

            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedObject = JsonConvert.DeserializeObject<JsonObjectDto>(responseContent);

            Assert.That(updatedObject, Is.Not.Null, "The response should contain the updated JSON object.");
            Assert.That(updatedObject?.Data?["firstName"], Is.EqualTo("Petr"), "The value for 'firstName' should have been updated.");
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
