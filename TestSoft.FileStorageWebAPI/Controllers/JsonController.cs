using Microsoft.AspNetCore.Mvc;
using TestSoft.FileStorageWebAPI.Contracts;
using TestSoft.FileStorageWebAPI.Services;

namespace TestSoft.FileStorageWebAPI.Controllers
{
    [ApiController]
    [Route("api/json")]
    public class JsonController : ControllerBase
    {
        private readonly ILogger<JsonController> _logger;
        private readonly IJsonService _jsonService;

        public JsonController(ILogger<JsonController> logger, IJsonService jsonService)
        {
            _logger = logger;
            _jsonService = jsonService;
        }

        /// <summary>
        /// Apply Patch operations to a JSON object.
        /// </summary>
        /// <param name="id">The ID of the JSON object.</param>
        /// <param name="patchRequest">The list of Patch operations.</param>
        /// <returns>The updated object or an error.</returns>
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PatchJsonObjectAsync(Guid id, [FromBody] JsonPatchRequestDto patchRequest)
        {
            _logger.LogInformation("Started applying Patch operations to object with ID {Id}", id);

            var result = await _jsonService.ApplyPatchAsync(id, patchRequest.Operations);

            if (!result.Success)
            {
                _logger.LogWarning("Error applying Patch operations: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            _logger.LogInformation("Patch operations successfully applied to object with ID {Id}", id);
            return Ok(result.UpdatedObject);
        }

        /// <summary>
        /// Get a JSON object by ID.
        /// </summary>
        /// <param name="id">The ID of the JSON object.</param>
        /// <returns>The found object or NotFound.</returns>
        [HttpGet("{id:guid}", Name = "GetJsonObject")]
        public async Task<IActionResult> GetJsonObjectAsync(Guid id)
        {
            _logger.LogInformation("Retrieving JSON object with ID {Id}", id);

            var jsonObject = await _jsonService.GetByIdAsync(id);
            if (jsonObject == null)
            {
                _logger.LogWarning("Object with ID {Id} not found", id);
                return NotFound();
            }

            return Ok(jsonObject);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJsonObjectAsync([FromBody] JsonObjectDto jsonObject)
        {
            _logger.LogInformation("Adding new JSON object");

            var id = await _jsonService.AddAsync(jsonObject);
            return CreatedAtRoute("GetJsonObject", new { id }, jsonObject);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteJsonObject(Guid id)
        {
            _logger.LogInformation("Deleting JSON object with ID {Id}", id);

            var success = _jsonService.Delete(id);
            if (!success)
            {
                _logger.LogWarning("Object with ID {Id} not found for deletion", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
