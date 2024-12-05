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
        /// ��������� Patch-�������� � JSON-�������.
        /// </summary>
        /// <param name="id">������������� JSON-�������.</param>
        /// <param name="patchRequest">������ �������� Patch.</param>
        /// <returns>���������� ������ ��� ������.</returns>
        [HttpPatch("{id:guid}")]
        public IActionResult PatchJsonObject(Guid id, [FromBody] JsonPatchRequestDto patchRequest)
        {
            _logger.LogInformation("������ ���������� Patch-�������� � ������� � ID {Id}", id);

            var result = _jsonService.ApplyPatch(id, patchRequest.Operations);

            if (!result.Success)
            {
                _logger.LogWarning("������ ��� ���������� Patch-��������: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            _logger.LogInformation("Patch-�������� ������� ��������� � ������� � ID {Id}", id);
            return Ok(result.UpdatedObject);
        }

        /// <summary>
        /// �������� JSON-������ �� ID.
        /// </summary>
        /// <param name="id">������������� JSON-�������.</param>
        /// <returns>��������� ������ ��� NotFound.</returns>
        [HttpGet("{id:guid}", Name = "GetJsonObject")]
        public IActionResult GetJsonObject(Guid id)
        {
            _logger.LogInformation("��������� JSON-������� � ID {Id}", id);

            var jsonObject = _jsonService.GetById(id);
            if (jsonObject == null)
            {
                _logger.LogWarning("������ � ID {Id} �� ������", id);
                return NotFound();
            }

            return Ok(jsonObject);
        }

        [HttpPost]
        public IActionResult CreateJsonObject([FromBody] JsonObjectDto jsonObject)
        {
            _logger.LogInformation("���������� ������ JSON-�������");

            var id = _jsonService.Add(jsonObject);
            return CreatedAtRoute("GetJsonObject", new { id }, jsonObject);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteJsonObject(Guid id)
        {
            _logger.LogInformation("�������� JSON-������� � ID {Id}", id);

            var success = _jsonService.Delete(id);
            if (!success)
            {
                _logger.LogWarning("������ � ID {Id} �� ������ ��� ��������", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
