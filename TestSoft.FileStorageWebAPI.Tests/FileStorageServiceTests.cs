using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TestSoft.FileStorageLibrary.Contracts;
using TestSoft.FileStorageLibrary.Services;

namespace TestSoft.FileStorageLibrary.CRUD.Tests
{
    [TestFixture]
    public class FileStorageServiceTests
    {
        private FileStorageService _fileStorageService;
        private string _storageDirectory;
        private Mock<IFileSystem> _fileSystemMock;

        [SetUp]
        public void Setup()
        {
            _storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestStorage");

            _fileSystemMock = new Mock<IFileSystem>();
            _fileStorageService = new FileStorageService(_storageDirectory, _fileSystemMock.Object);

            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }

        [Test]
        public void AddOrUpdate_ShouldSaveFile_WhenDataIsValid()
        {
            var fileData = new FileDataDto
            {
                Id = Guid.NewGuid(),
                Data = new Dictionary<string, object> { { "key1", "value1" } }
            };

            _fileSystemMock.Setup(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

            _fileStorageService.AddOrUpdate(fileData);

            _fileSystemMock.Verify(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Test]
        public void Get_ShouldReturnFileData_WhenFileExists()
        {
            var fileId = Guid.NewGuid();
            var fileData = new FileDataDto
            {
                Id = fileId,
                Data = new Dictionary<string, object>
                {
                    { "key1", "value1" },
                    { "key2", 123 }
                }
            };

            var jsonString = JsonSerializer.Serialize(fileData);

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);
            _fileSystemMock.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns(jsonString);

            var result = _fileStorageService.Get(fileId);

            Assert.IsNotNull(result);
            Assert.AreEqual(fileId, result?.Id);
            Assert.AreEqual("value1".Trim(), result?.Data["key1"].ToString().Trim());
        }

        [Test]
        public void Get_ShouldReturnNull_WhenFileDoesNotExist()
        {
            var fileId = Guid.NewGuid();

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(false);

            var result = _fileStorageService.Get(fileId);

            Assert.IsNull(result);
        }

        [Test]
        public void Delete_ShouldReturnTrue_WhenFileIsDeleted()
        {
            var fileId = Guid.NewGuid();

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);
            _fileSystemMock.Setup(fs => fs.Delete(It.IsAny<string>()));

            var result = _fileStorageService.Delete(fileId);

            Assert.IsTrue(result);
        }

        [Test]
        public void Delete_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            var fileId = Guid.NewGuid();

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(false);

            var result = _fileStorageService.Delete(fileId);

            Assert.IsFalse(result);
        }
    }
}
