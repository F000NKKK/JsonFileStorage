using System.Text.Json;
using System.Collections.Concurrent;
using TestSoft.FileStorageLibrary.Contracts;
using System.Runtime.CompilerServices;

namespace TestSoft.FileStorageLibrary.Services
{

    public class FileStorageService : IFileStorageService
    {
        private readonly string _storageDirectory;
        private readonly IFileSystem _fileSystem;

        public FileStorageService(string storageDirectory, IFileSystem fileSystem)
        {
            _storageDirectory = storageDirectory;
            _fileSystem = fileSystem;
            Directory.CreateDirectory(_storageDirectory);
        }

        private string GetFilePath(Guid id) => Path.Combine(_storageDirectory, $"{id}.json.gz");

        public async Task<FileDataDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filePath = GetFilePath(id);
            if (!_fileSystem.Exists(filePath)) return null;

            var compressedData = await _fileSystem.ReadCompressedFileAsync(filePath, cancellationToken);
            return JsonSerializer.Deserialize<FileDataDto>(compressedData);
        }

        public async Task AddAsync(FileDataDto data, CancellationToken cancellationToken = default)
        {
            var filePath = GetFilePath(data.Id);
            if (_fileSystem.Exists(filePath))
                throw new InvalidOperationException($"File with ID {data.Id} already exists.");

            var serializedData = JsonSerializer.SerializeToUtf8Bytes(data);
            await _fileSystem.WriteCompressedFileAsync(filePath, serializedData, cancellationToken);
        }

        public async Task UpdateAsync(FileDataDto data, CancellationToken cancellationToken = default)
        {
            var filePath = GetFilePath(data.Id);
            if (!_fileSystem.Exists(filePath))
                throw new FileNotFoundException($"File with ID {data.Id} does not exist.");

            var serializedData = JsonSerializer.SerializeToUtf8Bytes(data);
            await _fileSystem.WriteCompressedFileAsync(filePath, serializedData, cancellationToken);
        }

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