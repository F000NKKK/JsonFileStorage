using System.Text.Json;
using TestSoft.FileStorageLibrary.Contracts;

namespace TestSoft.FileStorageLibrary.Services
{
    /// <summary>
    /// Provides services for storing, retrieving, and managing files in a file system.
    /// </summary>
    public class FileStorageService
    {
        private readonly string _storageDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageService"/> class.
        /// Creates the storage directory if it doesn't exist.
        /// </summary>
        /// <param name="storageDirectory">The directory where files will be stored.</param>
        public FileStorageService(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
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

            if (!File.Exists(filePath)) return null;

            var jsonString = File.ReadAllText(filePath);
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
            File.WriteAllText(filePath, jsonString); // Overwrite the file
        }

        /// <summary>
        /// Deletes the file corresponding to the given ID.
        /// </summary>
        /// <param name="id">The unique identifier for the file to delete.</param>
        /// <returns>True if the file was deleted, false if the file does not exist.</returns>
        public bool Delete(Guid id)
        {
            var filePath = GetFilePath(id);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
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
                var jsonString = File.ReadAllText(file);
                var data = JsonSerializer.Deserialize<FileDataDto>(jsonString);
                if (data != null)
                {
                    fileDataList.Add(data);
                }
            }

            return fileDataList;
        }
    }
}
