using System.Collections.Concurrent;
using TestSoft.FileStorageLibrary.Services;
using TestSoft.FileStorageLibrary.Contracts;
using TestSoft.FileStorageWebAPI.Contracts;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TestSoft.FileStorageWebAPI.Services
{
    public class JsonService : IJsonService
    {
        private readonly FileStorageService _fileStorageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonService"/> class.
        /// </summary>
        /// <param name="fileStorageService">The file storage service used to manage file operations.</param>
        public JsonService(FileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Adds a new JSON object and returns its unique identifier.
        /// </summary>
        /// <param name="jsonObject">The JSON object to be added.</param>
        /// <returns>The identifier of the added object.</returns>
        public Guid Add(JsonObjectDto jsonObject)
        {
            var id = Guid.NewGuid();
            var fileData = new FileDataDto { Id = id, Data = jsonObject.Data };

            _fileStorageService.AddOrUpdate(fileData);  // Saves or updates the file
            return id;
        }

        /// <summary>
        /// Deletes the JSON object identified by the provided identifier.
        /// </summary>
        /// <param name="id">The identifier of the JSON object to be deleted.</param>
        /// <returns>True if the object was successfully deleted, otherwise false.</returns>
        public bool Delete(Guid id)
        {
            return _fileStorageService.Delete(id);  // Deletes the file
        }

        /// <summary>
        /// Retrieves a JSON object by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the JSON object.</param>
        /// <returns>The JSON object if found, otherwise null.</returns>
        public JsonObjectDto GetById(Guid id)
        {
            var fileData = _fileStorageService.Get(id);
            return fileData != null ? new JsonObjectDto { Data = fileData.Data } : null!;
        }

        /// <summary>
        /// Applies a list of Patch operations to the JSON object identified by the provided identifier.
        /// </summary>
        /// <param name="id">The identifier of the JSON object.</param>
        /// <param name="operations">The list of Patch operations to apply.</param>
        /// <returns>A tuple containing success status, an error message (if any), and the updated JSON object.</returns>
        public (bool Success, string? ErrorMessage, JsonObjectDto UpdatedObject) ApplyPatch(Guid id, List<JsonPatchOperationDto> operations)
        {
            var fileData = _fileStorageService.Get(id);
            if (fileData == null)
            {
                return (false, "Object not found", null!);
            }

            var jsonNode = JsonSerializer.Deserialize<JsonNode>(JsonSerializer.Serialize(fileData.Data));
            if (jsonNode == null) return (false, "Invalid JSON structure.", null!);

            foreach (var operation in operations)
            {
                try
                {
                    ApplyOperation(jsonNode, operation);
                }
                catch (Exception ex)
                {
                    return (false, ex.Message, null!);
                }
            }

            fileData.Data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonNode.ToJsonString())!;
            _fileStorageService.AddOrUpdate(fileData);  // Updates the file with the new data
            return (true, null, new JsonObjectDto { Data = fileData.Data });
        }

        /// <summary>
        /// Applies a single Patch operation to a JSON node.
        /// </summary>
        /// <param name="jsonNode">The JSON node to modify.</param>
        /// <param name="operation">The Patch operation to apply.</param>
        /// <exception cref="Exception">Thrown if the path is not found or an unsupported operation is specified.</exception>
        private void ApplyOperation(JsonNode jsonNode, JsonPatchOperationDto operation)
        {
            var pathSegments = operation.Path.Trim('/').Split('/');
            var targetNode = jsonNode;

            // Traverse the path segments and find the target node
            for (int i = 0; i < pathSegments.Length - 1; i++)
            {
                targetNode = targetNode?[pathSegments[i]] ?? throw new Exception($"Path segment '{pathSegments[i]}' not found.");
            }

            var finalSegment = pathSegments[^1];

            // Apply the Patch operation (add, replace, or remove)
            switch (operation.Op.ToLower())
            {
                case "add":
                    targetNode![finalSegment] = JsonSerializer.SerializeToNode(operation.Value);
                    break;

                case "replace":
                    if (targetNode![finalSegment] == null)
                    {
                        throw new Exception($"Path '{operation.Path}' not found for replace operation.");
                    }
                    targetNode[finalSegment] = JsonSerializer.SerializeToNode(operation.Value);
                    break;

                case "remove":
                    if (!targetNode!.AsObject().Remove(finalSegment))
                    {
                        throw new Exception($"Path '{operation.Path}' not found for remove operation.");
                    }
                    break;

                default:
                    throw new Exception($"Unsupported operation '{operation.Op}'.");
            }
        }
    }
}
