using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSoft.FileStorageLibrary.Contracts;

namespace TestSoft.FileStorageLibrary.Services
{
    public interface IFileStorageService
    {
        void AddOrUpdate(FileDataDto fileData);
        bool Delete(Guid id);
        FileDataDto Get(Guid id);
        IEnumerable<FileDataDto> GetAll();
    }

}
