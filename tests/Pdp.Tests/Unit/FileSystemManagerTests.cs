using Moq;
using Pdp.App.Interfaces;
using Pdp.Inf.Interfaces;
using Pdp.Inf.Services;

namespace Pdp.Tests.Unit
{
    public class FileSystemManagerTests
    {
        [Fact]
        public async Task GetTreeAsync_ShouldBuildCorrectProjectStructure()
        {
            // Arrange - создаем структуру каталогов
            var rootPath = @"C:\Projects\MyApp";

            // Создаем мок для файловой системы
            var mockFsWrapper = new Mock<IFileSystemWrapper>();

            // Настраиваем проверку существования файлов
            mockFsWrapper
                .Setup(x => x.FileIsReadable(It.IsAny<string>()))
                .Returns(true);

            // Создаем мок для фабрики директорий
            var mockDirFactory = new Mock<IDirectoryInfoWrapper>();

            // Создаем структуру: 
            // MyApp/
            //   ├── Program.cs
            //   ├── Utils.cs
            //   ├── Models/
            //   │   └── User.cs
            //   └── Services/
            //       └── DataService.cs

            // Корневая директория
            var rootDir = new Mock<IDirectoryInfoWrapper>();
            rootDir.Setup(x => x.Name).Returns("MyApp");
            rootDir.Setup(x => x.FullName).Returns(rootPath);
            rootDir.Setup(x => x.IsDirectory).Returns(true);

            // Файлы в корне
            var file1 = CreateFileMock("Program.cs", Path.Combine(rootPath, "Program.cs"));
            var file2 = CreateFileMock("Utils.cs", Path.Combine(rootPath, "Utils.cs"));

            // Папка Models
            var modelsDir = CreateDirectoryMock("Models", Path.Combine(rootPath, "Models"));
            var userFile = CreateFileMock("User.cs", Path.Combine(rootPath, "Models", "User.cs"));
            modelsDir.Setup(x => x.GetFileSystemInfos()).Returns([userFile.Object]);

            // Папка Services
            var servicesDir = CreateDirectoryMock("Services", Path.Combine(rootPath, "Services"));
            var dataServiceFile = CreateFileMock("DataService.cs", Path.Combine(rootPath, "Services", "DataService.cs"));
            servicesDir.Setup(x => x.GetFileSystemInfos()).Returns([dataServiceFile.Object]);

            // Настраиваем корневую директорию
            rootDir.Setup(x => x.GetFileSystemInfos()).Returns([
                file1.Object,
                file2.Object,
                modelsDir.Object,
                servicesDir.Object
            ]);

            // Фабрика возвращает корневую директорию
            mockDirFactory
                .Setup(x => x.Create(rootPath))
                .Returns(rootDir.Object);

            // Мок для конфига
            var mockConfig = new Mock<IConfigService>();
            mockConfig
                .Setup(x => x.RootDirectory)
                .Returns(rootPath);

            // Создаем тестируемый сервис
            var fsm = new FileSystemManager(
                mockFsWrapper.Object,
                mockDirFactory.Object,
                mockConfig.Object);

            // Act - получаем структуру проекта
            var result = await fsm.GetTreeAsync();

            // Assert - проверяем структуру
            Assert.NotNull(result);
            Assert.Equal("MyApp", result.Name);
            Assert.Equal(rootPath, result.Path);
            Assert.True(result.IsDirectory);
            Assert.Equal(4, result.Children.Count); // 2 файла + 2 папки

            // 1. Проверяем файл Program.cs
            var programFile = result.Children.FirstOrDefault(x => x.Name == "Program.cs");
            Assert.NotNull(programFile);
            Assert.False(programFile.IsDirectory);
            Assert.Equal(Path.Combine(rootPath, "Program.cs"), programFile.Path);
            Assert.Empty(programFile.Children); // Файл не должен иметь детей

            // 2. Проверяем файл Utils.cs
            var utilsFile = result.Children.FirstOrDefault(x => x.Name == "Utils.cs");
            Assert.NotNull(utilsFile);
            Assert.False(utilsFile.IsDirectory);

            // 3. Проверяем папку Models
            var modelsDirResult = result.Children.FirstOrDefault(x => x.Name == "Models");
            Assert.NotNull(modelsDirResult);
            Assert.True(modelsDirResult.IsDirectory);
            Assert.Equal(Path.Combine(rootPath, "Models"), modelsDirResult.Path);
            Assert.Single(modelsDirResult.Children);

            var userFileResult = modelsDirResult.Children.FirstOrDefault(x => x.Name == "User.cs");
            Assert.NotNull(userFileResult);
            Assert.False(userFileResult.IsDirectory);
            Assert.Equal(Path.Combine(rootPath, "Models", "User.cs"), userFileResult.Path);

            // 4. Проверяем папку Services
            var servicesDirResult = result.Children.FirstOrDefault(x => x.Name == "Services");
            Assert.NotNull(servicesDirResult);
            Assert.True(servicesDirResult.IsDirectory);
            Assert.Single(servicesDirResult.Children);

            var dataServiceFileResult = servicesDirResult.Children.FirstOrDefault(x => x.Name == "DataService.cs");
            Assert.NotNull(dataServiceFileResult);
            Assert.False(dataServiceFileResult.IsDirectory);
            Assert.Equal(Path.Combine(rootPath, "Services", "DataService.cs"), dataServiceFileResult.Path);

            // 5. Проверяем родительские связи
            Assert.Equal(result, modelsDirResult.Parent);
            Assert.Equal(result, servicesDirResult.Parent);
            Assert.Equal(modelsDirResult, userFileResult.Parent);
            Assert.Equal(servicesDirResult, dataServiceFileResult.Parent);
        }

        [Fact]
        public async Task GetTreeAsync_WithEmptyDirectory_ShouldReturnOnlyRoot()
        {
            // Arrange
            var rootPath = @"C:\Projects\EmptyApp";

            var mockFsWrapper = new Mock<IFileSystemWrapper>();
            mockFsWrapper
                .Setup(x => x.FileIsReadable(It.IsAny<string>()))
                .Returns(true);

            var rootDir = new Mock<IDirectoryInfoWrapper>();
            rootDir.Setup(x => x.Name).Returns("EmptyApp");
            rootDir.Setup(x => x.FullName).Returns(rootPath);
            rootDir.Setup(x => x.IsDirectory).Returns(true);
            rootDir.Setup(x => x.GetFileSystemInfos()).Returns([]);

            var mockDirFactory = new Mock<IDirectoryInfoWrapper>();
            mockDirFactory
                .Setup(x => x.Create(rootPath))
                .Returns(rootDir.Object);

            var mockConfig = new Mock<IConfigService>();
            mockConfig
                .Setup(x => x.RootDirectory)
                .Returns(rootPath);

            var fsm = new FileSystemManager(
                mockFsWrapper.Object,
                mockDirFactory.Object,
                mockConfig.Object);

            // Act
            var result = await fsm.GetTreeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("EmptyApp", result.Name);
            Assert.True(result.IsDirectory);
            Assert.Empty(result.Children);
        }

        [Fact]
        public async Task GetTreeAsync_WithFileFilter_ShouldFilterByContent()
        {
            // Arrange
            var rootPath = @"C:\Projects\MyApp";
            var searchPattern = "Hello";

            var mockFsWrapper = new Mock<IFileSystemWrapper>();
            mockFsWrapper
                .Setup(x => x.FileIsReadable(It.IsAny<string>()))
                .Returns(true);

            // Создаем мок для чтения содержимого файлов
            var fsmMock = new Mock<IFileSystemManager>();
            fsmMock
                .Setup(x => x.ReadFileAsync(It.Is<string>(p => p.Contains("Program.cs"))))
                .ReturnsAsync("Console.WriteLine(\"Hello World\");");
            fsmMock
                .Setup(x => x.ReadFileAsync(It.Is<string>(p => p.Contains("Utils.cs"))))
                .ReturnsAsync("// No here");

            var rootDir = new Mock<IDirectoryInfoWrapper>();
            rootDir.Setup(x => x.Name).Returns("MyApp");
            rootDir.Setup(x => x.FullName).Returns(rootPath);
            rootDir.Setup(x => x.IsDirectory).Returns(true);

            var file1 = CreateFileMock("Program.cs", Path.Combine(rootPath, "Program.cs"));
            var file2 = CreateFileMock("Utils.cs", Path.Combine(rootPath, "Utils.cs"));

            rootDir.Setup(x => x.GetFileSystemInfos()).Returns([file1.Object, file2.Object]);

            var mockDirFactory = new Mock<IDirectoryInfoWrapper>();
            mockDirFactory
                .Setup(x => x.Create(rootPath))
                .Returns(rootDir.Object);

            var mockConfig = new Mock<IConfigService>();
            mockConfig
                .Setup(x => x.RootDirectory)
                .Returns(rootPath);

            var fsm = new FileSystemManager(
                mockFsWrapper.Object,
                mockDirFactory.Object,
                mockConfig.Object);

            // Подменяем метод ReadFileAsync для теста
            // Используем реальный FileSystemManager, но с замоканным IFileSystemWrapper
            // Для фильтрации по содержимому нам нужно, чтобы ReadFileAsync возвращал нужный контент
            mockFsWrapper
                .Setup(x => x.FileReadAsync(It.Is<string>(p => p.Contains("Program.cs"))))
                .ReturnsAsync("Console.WriteLine(\"Hello World\");");
            mockFsWrapper
                .Setup(x => x.FileReadAsync(It.Is<string>(p => p.Contains("Utils.cs"))))
                .ReturnsAsync("// No here");

            // Act
            var result = await fsm.GetTreeAsync(searchPattern);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children); // Только Program.cs
            Assert.Equal("Program.cs", result.Children[0].Name);
        }

        // Вспомогательный метод для создания мока файла
        private static Mock<IDirectoryInfoWrapper> CreateFileMock(string name, string fullPath)
        {
            var mock = new Mock<IDirectoryInfoWrapper>();
            mock.Setup(x => x.Name).Returns(name);
            mock.Setup(x => x.FullName).Returns(fullPath);
            mock.Setup(x => x.IsDirectory).Returns(false);
            mock.Setup(x => x.GetFileSystemInfos()).Returns([]);
            return mock;
        }

        // Вспомогательный метод для создания мока директории
        private static Mock<IDirectoryInfoWrapper> CreateDirectoryMock(string name, string fullPath)
        {
            var mock = new Mock<IDirectoryInfoWrapper>();
            mock.Setup(x => x.Name).Returns(name);
            mock.Setup(x => x.FullName).Returns(fullPath);
            mock.Setup(x => x.IsDirectory).Returns(true);
            mock.Setup(x => x.GetFileSystemInfos()).Returns([]);
            return mock;
        }
    }
}
