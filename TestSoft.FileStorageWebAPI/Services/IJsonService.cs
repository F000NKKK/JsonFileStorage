using TestSoft.FileStorageWebAPI.Contracts;

namespace TestSoft.FileStorageWebAPI.Services
{
    public interface IJsonService
    {
        /// <summary>
        /// Получить JSON-объект по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор объекта.</param>
        /// <returns>JSON-объект или null, если не найден.</returns>
        JsonObjectDto GetById(Guid id);

        /// <summary>
        /// Применить список Patch-операций к JSON-объекту.
        /// </summary>
        /// <param name="id">Идентификатор объекта.</param>
        /// <param name="operations">Список операций Patch.</param>
        /// <returns>Результат операции с обновлённым объектом.</returns>
        (bool Success, string? ErrorMessage, JsonObjectDto UpdatedObject) ApplyPatch(Guid id, List<JsonPatchOperationDto> operations);

        Guid Add(JsonObjectDto jsonObject); // Добавление объекта

        bool Delete(Guid id); // Удаление объекта
    }
}
