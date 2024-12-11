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
        public void Add_ShouldReturnNewGuid()
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

            _fileStorageServiceMock.Setup(f => f.AddOrUpdate(It.IsAny<FileDataDto>()));

            // Act
            var result = _jsonService.Add(jsonObject);

            // Assert
            Assert.IsInstanceOf<Guid>(result, "Add should return a new Guid.");
            _fileStorageServiceMock.Verify(f => f.AddOrUpdate(It.IsAny<FileDataDto>()), Times.Once);
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
            Assert.IsTrue(result, "Delete should return true when the object exists.");
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
            Assert.IsFalse(result, "Delete should return false when the object does not exist.");
        }

        [Test]
        public void GetById_ShouldReturnJsonObject_WhenObjectExists()
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
            _fileStorageServiceMock.Setup(f => f.Get(id)).Returns(fileData);

            // Act
            var result = _jsonService.GetById(id);

            // Assert
            Assert.IsNotNull(result, "GetById should return a JSON object when it exists.");
            Assert.AreEqual("value1", result.Data["key1"], "The data should match the expected object.");
        }

        [Test]
        public void GetById_ShouldReturnNull_WhenObjectDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fileStorageServiceMock.Setup(f => f.Get(id)).Returns((FileDataDto?)null);

            // Act
            var result = _jsonService.GetById(id);

            // Assert
            Assert.IsNull(result, "GetById should return null when the object does not exist.");
        }

        [Test]
        public void ApplyPatch_ShouldReturnUpdatedObject_WhenPatchIsSuccessful()
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

            _fileStorageServiceMock.Setup(f => f.Get(id)).Returns(fileData);

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
            var (success, error, updatedObject) = _jsonService.ApplyPatch(id, patchOperations);

            // Assert
            Assert.IsTrue(success, "ApplyPatch should succeed when the patch is valid.");
            Assert.IsNull(error, "There should be no error message.");
            Assert.AreEqual("newValue", updatedObject.Data["key1"].ToString(), "The value for 'key1' should be updated.");
            _fileStorageServiceMock.Verify(f => f.AddOrUpdate(It.IsAny<FileDataDto>()), Times.Once);
        }

        [Test]
        public void ApplyPatch_ShouldReturnError_WhenObjectNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fileStorageServiceMock.Setup(f => f.Get(id)).Returns((FileDataDto?)null);

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
            var (success, error, updatedObject) = _jsonService.ApplyPatch(id, patchOperations);

            // Assert
            Assert.IsFalse(success, "ApplyPatch should fail when the object is not found.");
            Assert.AreEqual("Object not found", error, "The error message should indicate the object was not found.");
        }

        [Test]
        public void ApplyPatch_ShouldReturnError_WhenInvalidPath()
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

            _fileStorageServiceMock.Setup(f => f.Get(id)).Returns(fileData);

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
            var (success, error, updatedObject) = _jsonService.ApplyPatch(id, patchOperations);

            // Assert
            Assert.IsFalse(success, "ApplyPatch should fail when the path is invalid.");
            Assert.AreEqual("Path '/key3' not found for replace operation.", error, "The error message should indicate the invalid path.");
        }

        [TearDown]
        public void TearDown()
        {
            _fileStorageServiceMock = null!;
            _jsonService = null!;
        }
    }
}