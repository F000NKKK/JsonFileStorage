using System.Text.Json;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using TestSoft.FileStorageLibrary.Contracts;

namespace TestSoft.FileStorageLibrary.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _storageDirectory;
        private readonly IFileSystem _fileSystem;
        private const long CompressionThreshold = 1024 * 1024; // 1 MB - размер файла, при котором его нужно сжать

        public FileStorageService(string storageDirectory, IFileSystem fileSystem)
        {
            _storageDirectory = storageDirectory;
            _fileSystem = fileSystem;
            Directory.CreateDirectory(_storageDirectory);
        }

        private string GetFilePath(Guid id, bool isCompressed) =>
            Path.Combine(_storageDirectory, $"{id}{(isCompressed ? ".json.gz" : ".json")}");

        public async Task<FileDataDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Сначала ищем сжатый файл
            var compressedFilePath = GetFilePath(id, true);
            if (_fileSystem.Exists(compressedFilePath))
            {
                var compressedData = await _fileSystem.ReadCompressedFileAsync(compressedFilePath, cancellationToken);
                return JsonSerializer.Deserialize<FileDataDto>(compressedData);
            }

            // Если сжатого нет, ищем обычный
            var filePath = GetFilePath(id, false);
            if (_fileSystem.Exists(filePath))
            {
                var fileData = await _fileSystem.ReadFileAsync(filePath, cancellationToken);
                return JsonSerializer.Deserialize<FileDataDto>(fileData);
            }

            return null;
        }

        public async Task AddAsync(FileDataDto data, CancellationToken cancellationToken = default)
        {
            var filePath = GetFilePath(data.Id, false);
            if (_fileSystem.Exists(filePath) || _fileSystem.Exists(GetFilePath(data.Id, true)))
                throw new InvalidOperationException($"File with ID {data.Id} already exists.");

            var serializedData = JsonSerializer.SerializeToUtf8Bytes(data);
            if (serializedData.Length > CompressionThreshold)
            {
                // Если размер больше порога, сжимаем файл
                await _fileSystem.WriteCompressedFileAsync(GetFilePath(data.Id, true), serializedData, cancellationToken);
            }
            else
            {
                // Иначе записываем обычный файл
                await _fileSystem.WriteFileAsync(GetFilePath(data.Id, false), serializedData, cancellationToken);
            }
        }

        public async Task UpdateAsync(FileDataDto data, CancellationToken cancellationToken = default)
        {
            var compressedFilePath = GetFilePath(data.Id, true);
            var filePath = GetFilePath(data.Id, false);

            var fileExists = _fileSystem.Exists(compressedFilePath) || _fileSystem.Exists(filePath);
            if (!fileExists)
                throw new FileNotFoundException($"File with ID {data.Id} does not exist.");

            var serializedData = JsonSerializer.SerializeToUtf8Bytes(data);

            // Запись нового файла, в зависимости от размера
            if (serializedData.Length > CompressionThreshold)
            {
                // Если размер больше порога, сжимаем файл
                await _fileSystem.WriteCompressedFileAsync(compressedFilePath, serializedData, cancellationToken);
                _fileSystem.Delete(filePath);
            }
            else
            {
                // Иначе записываем обычный файл
                await _fileSystem.WriteFileAsync(filePath, serializedData, cancellationToken);
                _fileSystem.Delete(compressedFilePath);
            }
        }


        public bool Delete(Guid id)
        {
            var compressedFilePath = GetFilePath(id, true);
            var filePath = GetFilePath(id, false);
            if (_fileSystem.Exists(compressedFilePath))
            {
                _fileSystem.Delete(compressedFilePath);
                return true;
            }
            else if (_fileSystem.Exists(filePath))
            {
                _fileSystem.Delete(filePath);
                return true;
            }
            return false;
        }

        public async IAsyncEnumerable<FileDataDto> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var files = await _fileSystem.GetFilesAsync(_storageDirectory, "*.json.gz", cancellationToken);
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var compressedData = await _fileSystem.ReadCompressedFileAsync(file, cancellationToken);
                var data = JsonSerializer.Deserialize<FileDataDto>(compressedData);
                if (data != null)
                {
                    yield return data;
                }
            }

            // Проверяем для обычных файлов
            var regularFiles = await _fileSystem.GetFilesAsync(_storageDirectory, "*.json", cancellationToken);
            foreach (var file in regularFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fileData = await _fileSystem.ReadFileAsync(file, cancellationToken);
                var data = JsonSerializer.Deserialize<FileDataDto>(fileData);
                if (data != null)
                {
                    yield return data;
                }
            }
        }

        public async Task<IEnumerable<FileDataDto>> SearchAsync(Func<FileDataDto, bool> predicate, CancellationToken cancellationToken = default)
        {
            var results = new ConcurrentBag<FileDataDto>();
            await Parallel.ForEachAsync(
                GetAllAsync(cancellationToken),
                cancellationToken,
                async (fileData, _) =>
                {
                    if (predicate(fileData))
                    {
                        results.Add(fileData);
                    }
                });
            return results;
        }

        public async Task UpdateFieldAsync(Guid id, Action<FileDataDto> updateAction, CancellationToken cancellationToken = default)
        {
            var fileData = await GetAsync(id, cancellationToken);
            if (fileData == null) return;
            updateAction(fileData);
            await UpdateAsync(fileData, cancellationToken);
        }

        public async Task DeleteByFilterAsync(Func<FileDataDto, bool> predicate, CancellationToken cancellationToken = default)
        {
            var toDelete = new ConcurrentBag<Guid>();
            await Parallel.ForEachAsync(
                GetAllAsync(cancellationToken),
                cancellationToken,
                async (fileData, _) =>
                {
                    if (predicate(fileData))
                    {
                        toDelete.Add(fileData.Id);
                    }
                });

            Parallel.ForEach(toDelete, id => Delete(id));
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var count = 0;
            await foreach (var _ in GetAllAsync(cancellationToken))
            {
                count++;
            }
            return count;
        }
    }
}
