using TestSoft.FileStorageWebAPI.Contracts;

namespace TestSoft.FileStorageWebAPI.Services
{
    public interface IJsonService
    {
        /// <summary>
        /// Get the JSON object by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the object.</param>
        /// <returns>The JSON object or null if not found.</returns>
        JsonObjectDto GetById(Guid id);

        /// <summary>
        /// Apply a list of Patch operations to the JSON object.
        /// </summary>
        /// <param name="id">The identifier of the object.</param>
        /// <param name="operations">The list of Patch operations.</param>
        /// <returns>The result of the operation with the updated object.</returns>
        /// <remarks>
        /// This method applies a series of Patch operations (add, replace, or remove) 
        /// to the specified JSON object identified by the provided <paramref name="id"/>.
        /// </remarks>
        (bool Success, string? ErrorMessage, JsonObjectDto UpdatedObject) ApplyPatch(Guid id, List<JsonPatchOperationDto> operations);

        /// <summary>
        /// Add a new JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object to add.</param>
        /// <returns>The identifier of the added object.</returns>
        Guid Add(JsonObjectDto jsonObject);

        /// <summary>
        /// Delete the JSON object by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the object to delete.</param>
        /// <returns>True if the object was deleted successfully, otherwise false.</returns>
        bool Delete(Guid id);
    }
}
