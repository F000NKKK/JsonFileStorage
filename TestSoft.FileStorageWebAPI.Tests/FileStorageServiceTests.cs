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

            // Используем мок для файловой системы
            _fileSystemMock = new Mock<IFileSystem>();
            _fileStorageService = new FileStorageService(_storageDirectory, _fileSystemMock.Object);

            // Создание директории, если не существует
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }

        [Test]
        public void AddOrUpdate_ShouldSaveFile_WhenDataIsValid()
        {
            // Данные для добавления или обновления
            var fileData = new FileDataDto
            {
                Id = Guid.NewGuid(),
                Data = new Dictionary<string, object> { { "key1", "value1" } }
            };

            // Мокаем поведение метода записи файла
            _fileSystemMock.Setup(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

            // Действие: вызываем метод AddOrUpdate
            _fileStorageService.AddOrUpdate(fileData);

            // Проверяем, что метод записи был вызван
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

            // Мокаем поведение чтения из файла
            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);
            _fileSystemMock.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns(jsonString);

            // Действие
            var result = _fileStorageService.Get(fileId);

            // Проверяем результат
            Assert.IsNotNull(result);
            Assert.AreEqual(fileId, result?.Id);
            Assert.AreEqual("value1".Trim(), result?.Data["key1"].ToString().Trim());
        }

        [Test]
        public void Get_ShouldReturnNull_WhenFileDoesNotExist()
        {
            var fileId = Guid.NewGuid();

            // Мокаем поведение, когда файл не существует
            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(false);

            // Действие
            var result = _fileStorageService.Get(fileId);

            // Проверяем результат
            Assert.IsNull(result);
        }

        [Test]
        public void Delete_ShouldReturnTrue_WhenFileIsDeleted()
        {
            var fileId = Guid.NewGuid();

            // Мокаем поведение файловой системы
            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(true);
            _fileSystemMock.Setup(fs => fs.Delete(It.IsAny<string>()));

            // Действие
            var result = _fileStorageService.Delete(fileId);

            // Проверяем результат
            Assert.IsTrue(result);
        }

        [Test]
        public void Delete_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            var fileId = Guid.NewGuid();

            // Мокаем поведение, когда файл не существует
            _fileSystemMock.Setup(fs => fs.Exists(It.IsAny<string>())).Returns(false);

            // Действие
            var result = _fileStorageService.Delete(fileId);

            // Проверяем результат
            Assert.IsFalse(result);
        }
    }
}
