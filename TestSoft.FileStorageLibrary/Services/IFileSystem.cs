namespace TestSoft.FileStorageLibrary.Services
{
    public interface IFileSystem
    {
        bool Exists(string path);
        Task<byte[]> ReadCompressedFileAsync(string path, CancellationToken cancellationToken = default);
        Task WriteCompressedFileAsync(string path, byte[] data, CancellationToken cancellationToken = default);
        void Delete(string path);
        string[] GetFiles(string path, string searchPattern);
        Task<string[]> GetFilesAsync(string path, string searchPattern, CancellationToken cancellationToken = default);
    }

}
