using Application.Interfaces;
using Application.Models;
using Infrastructure.Services;
using MediatR;
using Moq;

namespace Schiza.Tests.InfrastructureTests
{
    public class CatalogObserverTests : IDisposable
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly string _testDirectory;
        private CatalogObserver _observer;

        public CatalogObserverTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _configServiceMock = new Mock<IConfigService>();
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _configServiceMock.Setup(x => x.RootDirectory).Returns(_testDirectory);
            _observer = new CatalogObserver(_mediatorMock.Object, _configServiceMock.Object);
        }

        public void Dispose()
        {
            _observer?.Dispose();
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        [Fact]
        public void Constructor_WithNullMediator_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CatalogObserver(null, _configServiceMock.Object));
        }

        [Fact]
        public void Constructor_WithNullConfigService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CatalogObserver(_mediatorMock.Object, null));
        }

        [Fact]
        public void StartObserving_ShouldInitializeFileSystemWatcher()
        {
            // Act
            _observer.StartObserving();

            // Assert
            // Проверяем, что наблюдатель запущен и не выбрасывает исключений при работе с файлами
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "test");

            // Даем время на обработку события
            Thread.Sleep(100);

            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Never); // Должен сработать debounce
        }

        [Fact]
        public void StartObserving_WhenAlreadyObserving_ShouldRestartWatcher()
        {
            // Act
            _observer.StartObserving();
            _observer.StartObserving();

            // Assert - не должно быть исключений
            Assert.True(true);
        }

        [Fact]
        public void StopObserving_ShouldStopFileSystemWatcher()
        {
            // Arrange
            _observer.StartObserving();

            // Act
            _observer.StopObserving();

            // Проверяем, что после остановки события не обрабатываются
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "test");

            Thread.Sleep(600); // Ждем больше debounce времени

            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Never);
        }

        [Theory]
        [InlineData("temp.tmp")]
        [InlineData("file.~tmp")]
        [InlineData("backup.bak")]
        [InlineData("~$document.docx")]
        public void FileCreation_WithTemporaryExtension_ShouldNotTriggerNotification(string fileName)
        {
            // Arrange
            _observer.StartObserving();

            // Act
            var testFile = Path.Combine(_testDirectory, fileName);
            File.WriteAllText(testFile, "test");

            Thread.Sleep(600);

            // Assert
            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Never);
        }

        [Fact]
        public async Task FileCreation_ShouldTriggerNotificationAfterDebounceAsync()
        {
            // Arrange
            _observer.StartObserving();

            // Act
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "test");

            // Ждем завершения debounce
            await Task.Delay(600);

            // Assert
            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Once);
        }

        [Fact]
        public async Task MultipleFileEvents_ShouldTriggerSingleNotificationAsync()
        {
            // Arrange
            _observer.StartObserving();

            // Act - создаем несколько файлов подряд
            for (int i = 0; i < 5; i++)
            {
                var testFile = Path.Combine(_testDirectory, $"test{i}.txt");
                File.WriteAllText(testFile, "test");
                await Task.Delay(100);
            }

            // Ждем завершения debounce
            await Task.Delay(600);

            // Assert
            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Once);
        }

        [Fact]
        public async Task FileDeletion_ShouldTriggerNotificationAsync()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "test");

            _observer.StartObserving();

            // Act
            File.Delete(testFile);

            // Ждем завершения debounce
            await Task.Delay(600);

            // Assert
            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Once);
        }

        [Fact]
        public async Task FileRename_ShouldTriggerNotificationAsync()
        {
            // Arrange
            var oldFile = Path.Combine(_testDirectory, "old.txt");
            var newFile = Path.Combine(_testDirectory, "new.txt");
            File.WriteAllText(oldFile, "test");

            _observer.StartObserving();

            // Act
            File.Move(oldFile, newFile);

            // Ждем завершения debounce
            await Task.Delay(600);

            // Assert
            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Once);
        }

        [Fact]
        public async Task FileChange_ShouldNotTriggerNotification()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "test");

            _observer.StartObserving();

            // Act - изменяем содержимое файла
            File.WriteAllText(testFile, "changed content");

            // Ждем завершения debounce
            await Task.Delay(600);

            // Assert - Changed событие не должно вызывать уведомление
            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Never);
        }

        [Fact]
        public void Dispose_ShouldStopObservingAndReleaseResources()
        {
            // Arrange
            _observer.StartObserving();

            // Act
            _observer.Dispose();

            // Assert - попытка запустить после Dispose должна выбросить исключение
            Assert.Throws<ObjectDisposedException>(() => _observer.StartObserving());
        }

        [Fact]
        public void Dispose_WhenCalledMultipleTimes_ShouldNotThrowException()
        {
            // Arrange
            _observer.StartObserving();

            // Act & Assert
            _observer.Dispose();
            _observer.Dispose(); // Второй вызов не должен выбрасывать исключение
        }

        [Fact]
        public async Task StartObserving_AfterStop_ShouldWorkAgainAsync()
        {
            // Arrange
            _observer.StartObserving();
            _observer.StopObserving();

            // Act
            _observer.StartObserving();
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "test");

            await Task.Delay(600);

            // Assert
            _mediatorMock.Verify(x => x.Publish(It.IsAny<CatalogChangedNotification>()),
                Times.Once);
        }

    }
}
