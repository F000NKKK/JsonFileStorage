using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public async Task Add_ShouldSaveFile_WhenDataIsValidAsync()
        {
            // Arrange
            var fileData = new FileDataDto
            {
                Id = Guid.NewGuid(),
                Data = new Dictionary<string, object> { { "key1", "value1" } }
            };

            _fileSystemMock.Setup(fs => fs.WriteFileAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));

            // Act
            await _fileStorageService.AddAsync(fileData);

            // Assert
            _fileSystemMock.Verify(fs => fs.WriteFileAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Get_ShouldReturnFileData_WhenFileExistsAsync()
        {
            // Arrange
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
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);
            _fileSystemMock.Setup(fs => fs.ReadCompressedFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(jsonBytes);

            // Act
            var result = await _fileStorageService.GetAsync(fileId);

            // Assert
            Assert.That(result, Is.Not.Null, "Get should return a file when the file exists.");
            Assert.That(result?.Id, Is.EqualTo(fileId), "The file ID should match the expected ID.");
            Assert.That(result?.Data["key1"]?.ToString()?.Trim(), Is.EqualTo("value1".Trim()), "The 'key1' value should match the expected value.");
        }

        [Test]
        public async Task Get_ShouldReturnNull_WhenFileDoesNotExistAsync()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(false);

            // Act
            var result = await _fileStorageService.GetAsync(fileId);

            // Assert
            Assert.That(result, Is.Null, "Get should return null when the file does not exist.");
        }

        [Test]
        public void Delete_ShouldReturnTrue_WhenFileIsDeleted()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);
            _fileSystemMock.Setup(fs => fs.Delete(It.IsAny<string>()));

            // Act
            var result = _fileStorageService.Delete(fileId);

            // Assert
            Assert.That(result, Is.True, "Delete should return true when the file is deleted.");
        }

        [Test]
        public void Delete_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(false);

            // Act
            var result = _fileStorageService.Delete(fileId);

            // Assert
            Assert.That(result, Is.False, "Delete should return false when the file does not exist.");
        }
    }
}
