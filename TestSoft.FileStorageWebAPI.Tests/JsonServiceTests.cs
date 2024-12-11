using Moq;
using NUnit.Framework;
using TestSoft.FileStorageLibrary.Contracts;
using TestSoft.FileStorageLibrary.Services;
using TestSoft.FileStorageWebAPI.Contracts;
using TestSoft.FileStorageWebAPI.Services;
using System;
using System.Collections.Generic;

namespace TestSoft.FileStorageWebAPI.CRUD.Tests
{
    public class JsonServiceTests
    {
        private Mock<IFileStorageService> _fileStorageServiceMock;
        private JsonService _jsonService;

        [SetUp]
        public void Setup()
        {
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _jsonService = new JsonService(_fileStorageServiceMock.Object);
        }

        [Test]
        public async Task Add_ShouldReturnNewGuidAsync()
        {
            // Arrange
            var jsonObject = new JsonObjectDto
            {
                Data = new Dictionary<string, object>
                {
                    { "key1", "value1" },
                    { "key2", 123 },
                    { "key3", true }
                }
            };

            _fileStorageServiceMock.Setup(f => f.AddAsync(It.IsAny<FileDataDto>(), It.IsAny<CancellationToken>()));

            // Act
            var result = await _jsonService.AddAsync(jsonObject);

            // Assert
            Assert.That(result, Is.InstanceOf<Guid>(), "Add should return a new Guid.");
            _fileStorageServiceMock.Verify(f => f.AddAsync(It.IsAny<FileDataDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Delete_ShouldReturnTrue_WhenObjectExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fileStorageServiceMock.Setup(f => f.Delete(id)).Returns(true);

            // Act
            var result = _jsonService.Delete(id);

            // Assert
            Assert.That(result, Is.True, "Delete should return true when the object exists.");
            _fileStorageServiceMock.Verify(f => f.Delete(id), Times.Once);
        }

        [Test]
        public void Delete_ShouldReturnFalse_WhenObjectDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fileStorageServiceMock.Setup(f => f.Delete(id)).Returns(false);

            // Act
            var result = _jsonService.Delete(id);

            // Assert
            Assert.That(result, Is.False, "Delete should return false when the object does not exist.");
        }

        [Test]
        public async Task GetById_ShouldReturnJsonObject_WhenObjectExistsAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fileData = new FileDataDto
            {
                Id = id,
                Data = new Dictionary<string, object>
                {
                    { "key1", "value1" },
                    { "key2", 123 }
                }
            };
            _fileStorageServiceMock.Setup(f => f.GetAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(fileData);

            // Act
            var result = await _jsonService.GetByIdAsync(id);

            // Assert
            Assert.That(result, Is.Not.Null, "GetById should return a JSON object when it exists.");
            Assert.That(result?.Data?["key1"], Is.EqualTo("value1"), "The data should match the expected object.");
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenObjectDoesNotExistAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fileStorageServiceMock.Setup(f => f.GetAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((FileDataDto?)null);

            // Act
            var result = await _jsonService.GetByIdAsync(id);

            // Assert
            Assert.That(result, Is.Null, "GetById should return null when the object does not exist.");
        }

        [Test]
        public async Task ApplyPatch_ShouldReturnUpdatedObject_WhenPatchIsSuccessfulAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var initialData = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 123 }
            };

            var fileData = new FileDataDto
            {
                Id = id,
                Data = initialData
            };

            _fileStorageServiceMock.Setup(f => f.GetAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(fileData);

            var patchOperations = new List<JsonPatchOperationDto>
            {
                new JsonPatchOperationDto
                {
                    Op = "replace",
                    Path = "/key1",
                    Value = "newValue"
                }
            };

            // Act
            var (success, error, updatedObject) = await _jsonService.ApplyPatchAsync(id, patchOperations);

            // Assert
            Assert.That(success, Is.True, "ApplyPatch should succeed when the patch is valid.");
            Assert.That(error, Is.Null, "There should be no error message.");
            Assert.That(updatedObject?.Data?["key1"].ToString(), Is.EqualTo("newValue"), "The value for 'key1' should be updated.");
            _fileStorageServiceMock.Verify(f => f.UpdateAsync(It.IsAny<FileDataDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ApplyPatch_ShouldReturnError_WhenObjectNotFoundAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fileStorageServiceMock.Setup(f => f.GetAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((FileDataDto?)null);

            var patchOperations = new List<JsonPatchOperationDto>
            {
                new JsonPatchOperationDto
                {
                    Op = "replace",
                    Path = "/key1",
                    Value = "newValue"
                }
            };

            // Act
            var (success, error, updatedObject) = await _jsonService.ApplyPatchAsync(id, patchOperations);

            // Assert
            Assert.That(success, Is.False, "ApplyPatch should fail when the object is not found.");
            Assert.That(error, Is.EqualTo("Object not found"), "The error message should indicate the object was not found.");
        }

        [Test]
        public async Task ApplyPatch_ShouldReturnError_WhenInvalidPathAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var initialData = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 123 }
            };

            var fileData = new FileDataDto
            {
                Id = id,
                Data = initialData
            };

            _fileStorageServiceMock.Setup(f => f.GetAsync(id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(fileData);

            var patchOperations = new List<JsonPatchOperationDto>
            {
                new JsonPatchOperationDto
                {
                    Op = "replace",
                    Path = "/key3",  // Non-existent key
                    Value = "newValue"
                }
            };

            // Act
            var (success, error, updatedObject) = await _jsonService.ApplyPatchAsync(id, patchOperations);

            // Assert
            Assert.That(success, Is.False, "ApplyPatch should fail when the path is invalid.");
            Assert.That(error, Is.EqualTo("Path '/key3' not found for replace operation."), "The error message should indicate the invalid path.");
        }

        [TearDown]
        public void TearDown()
        {
            _fileStorageServiceMock = null!;
            _jsonService = null!;
        }
    }
}
