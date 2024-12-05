using System.Text.Json;
using TestSoft.FileStorageLibrary.Contracts;

namespace TestSoft.FileStorageLibrary.Services
{
    /// <summary>
    /// Provides services for storing, retrieving, and managing files in a file system.
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly string _storageDirectory;
        private readonly IFileSystem _fileSystem;
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageService"/> class.
        /// Creates the storage directory if it doesn't exist.
        /// </summary>
        /// <param name="storageDirectory">The directory where files will be stored.</param>
        public FileStorageService(string storageDirectory, IFileSystem fileSystem)
        {
            _storageDirectory = storageDirectory;
            _fileSystem = fileSystem;
            Directory.CreateDirectory(_storageDirectory); // Create directory for storing files if it doesn't exist
        }

        /// <summary>
        /// Generates the file path for a given ID.
        /// </summary>
        /// <param name="id">The unique identifier for the file.</param>
        /// <returns>The file path of the file corresponding to the ID.</returns>
        private string GetFilePath(Guid id)
        {
            // Create a unique file name based on GUID
            var fileName = $"{id.ToString()}.json";
            return Path.Combine(_storageDirectory, fileName);
        }

        /// <summary>
        /// Retrieves data from a file by ID.
        /// </summary>
        /// <param name="id">The unique identifier for the file.</param>
        /// <returns>The file data or null if the file does not exist.</returns>
        public FileDataDto? Get(Guid id)
        {
            var filePath = GetFilePath(id);

            if (!_fileSystem.Exists(filePath)) return null;

            var jsonString = _fileSystem.ReadAllText(filePath);
            return JsonSerializer.Deserialize<FileDataDto>(jsonString);
        }

        /// <summary>
        /// Adds or updates data in a file.
        /// </summary>
        /// <param name="data">The data to add or update in the file.</param>
        public void AddOrUpdate(FileDataDto data)
        {
            var filePath = GetFilePath(data.Id);
            var jsonString = JsonSerializer.Serialize(data);
            _fileSystem.WriteAllText(filePath, jsonString); // Overwrite the file
        }

        /// <summary>
        /// Deletes the file corresponding to the given ID.
        /// </summary>
        /// <param name="id">The unique identifier for the file to delete.</param>
        /// <returns>True if the file was deleted, false if the file does not exist.</returns>
        public bool Delete(Guid id)
        {
            var filePath = GetFilePath(id);
            if (_fileSystem.Exists(filePath))
            {
                _fileSystem.Delete(filePath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves all files in the storage directory.
        /// </summary>
        /// <returns>A collection of <see cref="FileDataDto"/> objects representing all files.</returns>
        public IEnumerable<FileDataDto> GetAll()
        {
            var files = Directory.GetFiles(_storageDirectory, "*.json");
            var fileDataList = new List<FileDataDto>();

            foreach (var file in files)
            {
                string jsonString;

                try
                {
                    jsonString = _fileSystem.ReadAllText(file); // Чтение данных из файла
                }
                catch (IOException ex)
                {
                    // Логируем ошибку, если не удается прочитать файл
                    Console.WriteLine($"Error reading file {file}: {ex.Message}");
                    continue; // Пропускаем файл, если ошибка чтения
                }

                if (string.IsNullOrEmpty(jsonString))
                {
                    // Пропускаем пустые файлы
                    Console.WriteLine($"File {file} is empty, skipping.");
                    continue;
                }

                try
                {
                    var data = JsonSerializer.Deserialize<FileDataDto>(jsonString); // Десериализация
                    if (data != null)
                    {
                        fileDataList.Add(data); // Добавляем только валидные данные
                    }
                    else
                    {
                        // Логируем, если десериализация вернула null
                        Console.WriteLine($"Failed to deserialize file {file}, skipping.");
                    }
                }
                catch (JsonException ex)
                {
                    // Логируем ошибку десериализации
                    Console.WriteLine($"Error deserializing file {file}: {ex.Message}");
                    continue; // Пропускаем файл с некорректными данными
                }
            }

            return fileDataList;
        }

    }
}
