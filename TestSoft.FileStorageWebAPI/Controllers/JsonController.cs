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
        /// Применить Patch-операции к JSON-объекту.
        /// </summary>
        /// <param name="id">Идентификатор JSON-объекта.</param>
        /// <param name="patchRequest">Список операций Patch.</param>
        /// <returns>Обновлённый объект или ошибка.</returns>
        [HttpPatch("{id:guid}")]
        public IActionResult PatchJsonObject(Guid id, [FromBody] JsonPatchRequestDto patchRequest)
        {
            _logger.LogInformation("Начало применения Patch-операций к объекту с ID {Id}", id);

            var result = _jsonService.ApplyPatch(id, patchRequest.Operations);

            if (!result.Success)
            {
                _logger.LogWarning("Ошибка при применении Patch-операций: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            _logger.LogInformation("Patch-операции успешно применены к объекту с ID {Id}", id);
            return Ok(result.UpdatedObject);
        }

        /// <summary>
        /// Получить JSON-объект по ID.
        /// </summary>
        /// <param name="id">Идентификатор JSON-объекта.</param>
        /// <returns>Найденный объект или NotFound.</returns>
        [HttpGet("{id:guid}", Name = "GetJsonObject")]
        public IActionResult GetJsonObject(Guid id)
        {
            _logger.LogInformation("Получение JSON-объекта с ID {Id}", id);

            var jsonObject = _jsonService.GetById(id);
            if (jsonObject == null)
            {
                _logger.LogWarning("Объект с ID {Id} не найден", id);
                return NotFound();
            }

            return Ok(jsonObject);
        }

        [HttpPost]
        public IActionResult CreateJsonObject([FromBody] JsonObjectDto jsonObject)
        {
            _logger.LogInformation("Добавление нового JSON-объекта");

            var id = _jsonService.Add(jsonObject);
            return CreatedAtRoute("GetJsonObject", new { id }, jsonObject);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteJsonObject(Guid id)
        {
            _logger.LogInformation("Удаление JSON-объекта с ID {Id}", id);

            var success = _jsonService.Delete(id);
            if (!success)
            {
                _logger.LogWarning("Объект с ID {Id} не найден для удаления", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
