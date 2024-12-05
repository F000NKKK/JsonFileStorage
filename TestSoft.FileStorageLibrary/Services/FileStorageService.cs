using System.Text.Json;
using TestSoft.FileStorageLibrary.Contracts;

namespace TestSoft.FileStorageLibrary.Services
{
    public class FileStorageService
    {
        private readonly string _storageDirectory;

        public FileStorageService(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
            Directory.CreateDirectory(_storageDirectory); // Создание директории для хранения файлов
        }

        private string GetFilePath(Guid id)
        {
            // Создание уникального имени файла на основе GUID
            var fileName = $"{id.ToString()}.json";
            return Path.Combine(_storageDirectory, fileName);
        }

        /// <summary>
        /// Получить данные по ID из файла.
        /// </summary>
        public FileDataDto? Get(Guid id)
        {
            var filePath = GetFilePath(id);

            if (!File.Exists(filePath)) return null;

            var jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<FileDataDto>(jsonString);
        }

        /// <summary>
        /// Добавить или обновить данные в файле.
        /// </summary>
        public void AddOrUpdate(FileDataDto data)
        {
            var filePath = GetFilePath(data.Id);
            var jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, jsonString); // Перезаписать файл
        }

        /// <summary>
        /// Удалить файл.
        /// </summary>
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
        /// Получить все файлы в директории.
        /// </summary>
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
