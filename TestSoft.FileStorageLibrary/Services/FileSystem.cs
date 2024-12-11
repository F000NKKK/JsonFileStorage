using System.IO.Compression;

namespace TestSoft.FileStorageLibrary.Services
{
    public class FileSystem : IFileSystem
    {
        public bool Exists(string path) => File.Exists(path);

        public async Task<byte[]> ReadCompressedFileAsync(string path, CancellationToken cancellationToken = default)
        {
            await using var fileStream = File.OpenRead(path);
            using var decompressionStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var memoryStream = new MemoryStream();
            await decompressionStream.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
        }

        public async Task WriteCompressedFileAsync(string path, byte[] data, CancellationToken cancellationToken = default)
        {
            await using var fileStream = File.Create(path);
            using var compressionStream = new GZipStream(fileStream, CompressionMode.Compress);
            await compressionStream.WriteAsync(data, cancellationToken);
        }

        public void Delete(string path) => File.Delete(path);

        public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

        public async Task<string[]> GetFilesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => Directory.GetFiles(path, searchPattern), cancellationToken);
        }
    }
}
