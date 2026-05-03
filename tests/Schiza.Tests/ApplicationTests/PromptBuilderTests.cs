using Application.Interfaces;
using Application.Models;
using Application.Services;
using Moq;

namespace Schiza.Tests.ApplicationTests
{
    public class PromptBuilderTests
    {
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly Mock<IFileSystemManager> _fileSystemManagerMock;
        private readonly PromptBuilder _builder;

        public PromptBuilderTests()
        {
            _configServiceMock = new Mock<IConfigService>();
            _fileSystemManagerMock = new Mock<IFileSystemManager>();
            _builder = new PromptBuilder(_configServiceMock.Object, _fileSystemManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldBuildPromptCorrectly_WhenValidRequest()
        {
            // Arrange
            var filePaths = new List<string> { "file1.txt", "file2.txt" };
            var queryText = "Test query";

            var config = new CombinationConfig
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "##intro####rules####query####context##",
                FileStructure = "File: ##path##\n##content##",
                IntroductionText = "Welcome",
                RulesText = "Be respectful"
            };

            _configServiceMock.Setup(cs => cs.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("file1.txt"))
                .ReturnsAsync("Content 1");
            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("file2.txt"))
                .ReturnsAsync("Content 2");

            // Act
            var result = await _builder.GetPrompt(filePaths, queryText);

            // Assert
            Assert.Contains("Welcome", result);
            Assert.Contains("Be respectful", result);
            Assert.Contains("Test query", result);
            Assert.Contains("File: file1.txt", result);
            Assert.Contains("Content 1", result);
            Assert.Contains("File: file2.txt", result);
            Assert.Contains("Content 2", result);

            _configServiceMock.Verify(cs => cs.GetCombinationConfigAsync(), Times.Once);
            _fileSystemManagerMock.Verify(fsm => fsm.ReadFileAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_ShouldCallReadFileAsyncForEachFilePath()
        {
            // Arrange
            var filePaths = new List<string> { "path1", "path2", "path3" };

            var config = new CombinationConfig
            {
                FileStructure = "##path##: ##content##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "##context##"
                // остальные поля могут быть пустыми или с дефолтами
            };

            _configServiceMock.Setup(cs => cs.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            foreach (var path in filePaths)
            {
                _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync(path))
                    .ReturnsAsync($"Content of {path}");
            }

            // Act
            var result = await _builder.GetPrompt(filePaths, "query");

            // Assert
            foreach (var path in filePaths)
            {
                _fileSystemManagerMock.Verify(fsm => fsm.ReadFileAsync(path), Times.Once);
            }
        }

        [Fact]
        public async Task Handle_ShouldHandleEmptyFileList()
        {
            // Arrange
            var filePaths = new List<string>();

            var config = new CombinationConfig
            {
                ContextKey = "##context##",
                QueryKey = "##query##",
                PromptStructure = "##query####context##",
                FileStructure = "",
                // прочие поля
            };

            _configServiceMock.Setup(cs => cs.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            // Act
            var result = await _builder.GetPrompt(filePaths, "query");

            // Assert
            Assert.Contains("query", result);
            // Проверяем, что ReadFileAsync не вызывался
            _fileSystemManagerMock.Verify(fsm => fsm.ReadFileAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReadFilesWithCorrectPaths()
        {
            // Arrange
            var filePaths = new List<string> { "docs/readme.md", "src/main.cs" };

            var config = new CombinationConfig
            {
                FileStructure = "##path## ##content##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "##context##"
            };

            _configServiceMock.Setup(cs => cs.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("docs/readme.md"))
                .ReturnsAsync("# Readme");
            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("src/main.cs"))
                .ReturnsAsync("class Program {}");

            // Act
            await _builder.GetPrompt(filePaths, "analyze");

            // Assert
            _fileSystemManagerMock.Verify(fsm => fsm.ReadFileAsync("docs/readme.md"), Times.Once);
            _fileSystemManagerMock.Verify(fsm => fsm.ReadFileAsync("src/main.cs"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUseConfigValuesCorrectly()
        {
            // Arrange
            var filePaths = new List<string> { "test.txt" };
            var queryText = "custom query";

            var expectedIntroKey = "[[INTRO]]";
            var expectedRulesKey = "[[RULES]]";
            var expectedQueryKey = "[[USER_QUERY]]";
            var expectedContextKey = "[[FILES]]";
            var expectedFilePathKey = "[[FILE_PATH]]";
            var expectedFileContentKey = "[[FILE_CONTENT]]";
            var expectedPromptStructure = "[[INTRO]]\n[[RULES]]\n[[USER_QUERY]]\n[[FILES]]";
            var expectedFileStructure = "Path: [[FILE_PATH]]\nContent: [[FILE_CONTENT]]";
            var expectedIntroText = "System instructions";
            var expectedRulesText = "Coding rules";

            var config = new CombinationConfig
            {
                IntroductionKey = expectedIntroKey,
                RulesKey = expectedRulesKey,
                QueryKey = expectedQueryKey,
                ContextKey = expectedContextKey,
                FilePathKey = expectedFilePathKey,
                FileContentKey = expectedFileContentKey,
                PromptStructure = expectedPromptStructure,
                FileStructure = expectedFileStructure,
                IntroductionText = expectedIntroText,
                RulesText = expectedRulesText
            };

            _configServiceMock.Setup(cs => cs.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("test.txt"))
                .ReturnsAsync("file content");

            // Act
            var result = await _builder.GetPrompt(filePaths, queryText);

            // Assert
            Assert.Contains(expectedIntroText, result);
            Assert.Contains(expectedRulesText, result);
            Assert.Contains(queryText, result);
            Assert.Contains("test.txt", result);
            Assert.Contains("file content", result);
            // Проверяем, что ключи были заменены
            Assert.DoesNotContain(expectedFilePathKey, result);
            Assert.DoesNotContain(expectedFileContentKey, result);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenFileNotFound()
        {
            // Arrange
            var filePaths = new List<string> { "missing.txt" };

            var config = new CombinationConfig
            {
                FileStructure = "##path## ##content##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "##context##"
            };

            _configServiceMock.Setup(cs => cs.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("missing.txt"))
                .ThrowsAsync(new FileNotFoundException("File not found"));

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _builder.GetPrompt(filePaths, "query"));
        }

        [Fact]
        public async Task Handle_ShouldPreserveOrderOfFiles()
        {
            // Arrange
            var filePaths = new List<string> { "first.txt", "second.txt", "third.txt" };

            var config = new CombinationConfig
            {
                FileStructure = "File: ##path##\n##content##\n",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "##context##"
            };

            _configServiceMock.Setup(cs => cs.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("first.txt"))
                .ReturnsAsync("First content");
            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("second.txt"))
                .ReturnsAsync("Second content");
            _fileSystemManagerMock.Setup(fsm => fsm.ReadFileAsync("third.txt"))
                .ReturnsAsync("Third content");

            // Act
            var result = await _builder.GetPrompt(filePaths, "query");

            // Assert
            var firstIndex = result.IndexOf("first.txt");
            var secondIndex = result.IndexOf("second.txt");
            var thirdIndex = result.IndexOf("third.txt");

            Assert.True(firstIndex < secondIndex, "First file should appear before second");
            Assert.True(secondIndex < thirdIndex, "Second file should appear before third");
        }
    }
}