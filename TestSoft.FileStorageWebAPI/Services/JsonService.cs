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

        public JsonService(FileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public Guid Add(JsonObjectDto jsonObject)
        {
            var id = Guid.NewGuid();
            var fileData = new FileDataDto { Id = id, Data = jsonObject.Data };

            _fileStorageService.AddOrUpdate(fileData);  // Сохраняем или обновляем файл
            return id;
        }

        public bool Delete(Guid id)
        {
            return _fileStorageService.Delete(id);  // Удаляем файл
        }

        public JsonObjectDto GetById(Guid id)
        {
            var fileData = _fileStorageService.Get(id);
            return fileData != null ? new JsonObjectDto { Data = fileData.Data } : null!;
        }

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
            _fileStorageService.AddOrUpdate(fileData);  // Обновляем файл с новыми данными
            return (true, null, new JsonObjectDto { Data = fileData.Data });
        }

        private void ApplyOperation(JsonNode jsonNode, JsonPatchOperationDto operation)
        {
            var pathSegments = operation.Path.Trim('/').Split('/');
            var targetNode = jsonNode;

            for (int i = 0; i < pathSegments.Length - 1; i++)
            {
                targetNode = targetNode?[pathSegments[i]] ?? throw new Exception($"Path segment '{pathSegments[i]}' not found.");
            }

            var finalSegment = pathSegments[^1];

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
