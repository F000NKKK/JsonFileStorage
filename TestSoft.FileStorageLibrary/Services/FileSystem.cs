using System.IO.Compression;

namespace TestSoft.FileStorageLibrary.Services
{
    public class FileSystem : IFileSystem
    {
        public bool Exists(string path) => File.Exists(path);

        public async Task<byte[]> ReadCompressedFileAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var fileStream = File.OpenRead(path);
                using var decompressionStream = new GZipStream(fileStream, CompressionMode.Decompress);
                using var memoryStream = new MemoryStream();
                await decompressionStream.CopyToAsync(memoryStream, cancellationToken);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                // Логирование или обработка ошибок
                throw new IOException($"Error reading compressed file: {path}", ex);
            }
        }

        public async Task WriteCompressedFileAsync(string path, byte[] data, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var fileStream = File.Create(path);
                using var compressionStream = new GZipStream(fileStream, CompressionMode.Compress);
                await compressionStream.WriteAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                // Логирование или обработка ошибок
                throw new IOException($"Error writing compressed file: {path}", ex);
            }
        }

        public async Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var fileStream = File.OpenRead(path);
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                // Логирование или обработка ошибок
                throw new IOException($"Error reading file: {path}", ex);
            }
        }

        public async Task WriteFileAsync(string path, byte[] data, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var fileStream = File.Create(path);
                await fileStream.WriteAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                // Логирование или обработка ошибок
                throw new IOException($"Error writing file: {path}", ex);
            }
        }

        public void Delete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                // Логирование или обработка ошибок
                throw new IOException($"Error deleting file: {path}", ex);
            }
        }

        public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

        public async Task<string[]> GetFilesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Task.Run(() => Directory.GetFiles(path, searchPattern), cancellationToken);
            }
            catch (Exception ex)
            {
                // Логирование или обработка ошибок
                throw new IOException($"Error getting files in directory: {path}", ex);
            }
        }
    }
}
