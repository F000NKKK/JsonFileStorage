using System.Runtime.CompilerServices;
using TestSoft.FileStorageLibrary.Contracts;

namespace TestSoft.FileStorageLibrary.Services
{
    public interface IFileStorageService
    {
        Task<FileDataDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(FileDataDto data, CancellationToken cancellationToken = default);
        Task UpdateAsync(FileDataDto data, CancellationToken cancellationToken = default);
        bool Delete(Guid id);
        IAsyncEnumerable<FileDataDto> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
        Task<IEnumerable<FileDataDto>> SearchAsync(Func<FileDataDto, bool> predicate, CancellationToken cancellationToken = default);
        Task UpdateFieldAsync(Guid id, Action<FileDataDto> updateAction, CancellationToken cancellationToken = default);
        Task DeleteByFilterAsync(Func<FileDataDto, bool> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }

}
