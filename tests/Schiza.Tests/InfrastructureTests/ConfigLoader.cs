using System;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Infrastructure.Services;
using Moq;
using Xunit;
using Infrastructure.Interfaces;

namespace Schiza.Tests.InfrastructureTests
{
    public class ConfigLoaderTests
    {
        private readonly Mock<IFileSystemWrapper> _mockFs;
        private readonly Mock<IFileSystemConstants> _mockConstants;
        private readonly ConfigLoader _configLoader;
        private readonly string _testProjectDirectory = @"C:\test\project";
        private readonly string _appDirectory = @"C:\Users\test\AppData\Roaming\Schiza";

        public ConfigLoaderTests()
        {
            _mockFs = new Mock<IFileSystemWrapper>();
            _mockConstants = new Mock<IFileSystemConstants>();

            _mockConstants.Setup(c => c.ConfigFileName).Returns("config.json");
            _mockConstants.Setup(c => c.LocalFolderName).Returns(".schiza");
            _mockConstants.Setup(c => c.AppName).Returns("Schiza");

            _mockFs.Setup(f => f.GetApplicationDirectory()).Returns(_appDirectory);

            _configLoader = new ConfigLoader(_mockFs.Object, _mockConstants.Object);
        }

        #region LoadGlobal Tests

        [Fact]
        public async Task LoadGlobal_WhenConfigFileDoesNotExist_ReturnsNewConfig()
        {
            // Arrange
            var expectedPath = Path.Combine(_appDirectory, "config.json");
            _mockFs.Setup(f => f.FileExist(expectedPath)).Returns(false);

            // Act
            var result = await _configLoader.LoadGlobal();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Config>(result);
            Assert.Equal("##intro##", result.IntroductionKey);
            Assert.Equal("##context##", result.ContextKey);
            Assert.Equal("##rules##", result.RulesKey);
            Assert.Equal("##query##", result.QueryKey);
            Assert.Equal("##path##", result.FilePathKey);
            Assert.Equal("##content##", result.FileContentKey);

            _mockFs.Verify(f => f.FileReadAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoadGlobal_WhenConfigFileExistsAndIsValid_ReturnsDeserializedConfig()
        {
            // Arrange
            var expectedPath = Path.Combine(_appDirectory, "config.json");
            var expectedJson = @"
            {
                ""IntroductionKey"": ""custom_intro"",
                ""ContextKey"": ""custom_context"",
                ""RulesKey"": ""custom_rules"",
                ""PromptStructure"": ""custom structure""
            }";

            _mockFs.Setup(f => f.FileExist(expectedPath)).Returns(true);
            _mockFs.Setup(f => f.FileReadAsync(expectedPath)).ReturnsAsync(expectedJson);
            _mockFs.Setup(f => f.PathCombine(_appDirectory, "config.json")).Returns(expectedPath);
            // Act
            var result = await _configLoader.LoadGlobal();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("custom_intro", result.IntroductionKey);
            Assert.Equal("custom_context", result.ContextKey);
            Assert.Equal("custom_rules", result.RulesKey);
            Assert.Equal("custom structure", result.PromptStructure);

            _mockFs.Verify(f => f.FileReadAsync(expectedPath), Times.Once);
        }

        [Fact]
        public async Task LoadGlobal_WhenConfigFileExistsButDeserializationReturnsNull_ReturnsNewConfig()
        {
            // Arrange
            var expectedPath = Path.Combine(_appDirectory, "config.json");
            var nullJson = "null";

            _mockFs.Setup(f => f.FileExist(expectedPath)).Returns(true);
            _mockFs.Setup(f => f.FileReadAsync(expectedPath)).ReturnsAsync(nullJson);

            // Act
            var result = await _configLoader.LoadGlobal();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Config>(result);
        }

        #endregion

        #region LoadLocal Tests

        [Fact]
        public async Task LoadLocal_WhenConfigFileDoesNotExist_ReturnsNewConfig()
        {
            // Arrange
            var expectedPath = Path.Combine(_testProjectDirectory, ".schiza", "config.json");
            _mockFs.Setup(f => f.FileExist(expectedPath)).Returns(false);

            // Act
            var result = await _configLoader.LoadLocal(_testProjectDirectory);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Config>(result);
            Assert.Equal("##intro##", result.IntroductionKey);
            Assert.Equal("##context##", result.ContextKey);

            _mockFs.Verify(f => f.FileReadAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoadLocal_WhenConfigFileExistsAndIsValid_ReturnsDeserializedConfig()
        {
            // Arrange
            var expectedPath = Path.Combine(_testProjectDirectory, ".schiza", "config.json");
            var expectedJson = @"
            {
                ""IntroductionKey"": ""local_intro"",
                ""RulesKey"": ""local_rules"",
                ""QueryKey"": ""local_query""
            }";

            _mockFs.Setup(f => f.FileExist(expectedPath)).Returns(true);
            _mockFs.Setup(f => f.FileReadAsync(expectedPath)).ReturnsAsync(expectedJson);
            _mockFs.Setup(f => f.PathCombine(_testProjectDirectory, ".schiza", "config.json")).Returns(expectedPath);
            // Act
            var result = await _configLoader.LoadLocal(_testProjectDirectory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_intro", result.IntroductionKey);
            Assert.Equal("local_rules", result.RulesKey);
            Assert.Equal("local_query", result.QueryKey);

            _mockFs.Verify(f => f.FileReadAsync(expectedPath), Times.Once);
        }


        #endregion

        #region SaveLocal Tests

        [Fact]
        public async Task SaveLocal_ValidConfig_DeletesExistingAndSavesNew()
        {
            // Arrange
            var config = new Config
            {
                IntroductionKey = "saved_intro",
                RulesKey = "saved_rules",
                PromptStructure = "saved_structure"
            };
            var expectedPath = Path.Combine(_testProjectDirectory, ".schiza", "config.json");

            _mockFs.Setup(f => f.PathCombine(_testProjectDirectory, ".schiza", "config.json"))
                   .Returns(expectedPath);

            // Act
            await _configLoader.SaveLocal(config, _testProjectDirectory);

            // Assert
            _mockFs.Verify(f => f.FileDelete(expectedPath), Times.Once);
            _mockFs.Verify(f => f.FileWriteAsync(expectedPath, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SaveLocal_ConfigIsSerializedCorrectly()
        {
            // Arrange
            var config = new Config
            {
                IntroductionKey = "test_intro",
                ContextKey = "test_context",
                RulesKey = "test_rules",
                QueryKey = "test_query",
                FilePathKey = "test_path",
                FileContentKey = "test_content",
                PromptStructure = "test_prompt",
                FileStructure = "test_file",
                IntroductionDef = "test_intro_def",
                RulesDef = "test_rules_def"
            };

            var expectedPath = Path.Combine(_testProjectDirectory, ".schiza", "config.json");
            string serializedJson = null;

            _mockFs.Setup(f => f.FileWriteAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .Callback<string, string>((path, content) => serializedJson = content)
                   .Returns(Task.CompletedTask);

            // Act
            await _configLoader.SaveLocal(config, _testProjectDirectory);

            // Assert
            Assert.NotNull(serializedJson);
            Assert.Contains("test_intro", serializedJson);
            Assert.Contains("test_context", serializedJson);
            Assert.Contains("test_rules", serializedJson);
            Assert.Contains("test_query", serializedJson);
        }

        #endregion

        #region SaveGlobal Tests

        [Fact]
        public async Task SaveGlobal_ValidConfig_DeletesExistingAndSavesNew()
        {
            // Arrange
            var config = new Config
            {
                IntroductionKey = "global_intro",
                RulesKey = "global_rules"
            };
            var expectedPath = Path.Combine(_appDirectory, "config.json");

            _mockFs.Setup(f => f.PathCombine(_appDirectory, "config.json"))
                   .Returns(expectedPath);

            // Act
            await _configLoader.SaveGlobal(config);

            // Assert
            _mockFs.Verify(f => f.FileDelete(expectedPath), Times.Once);
            _mockFs.Verify(f => f.FileWriteAsync(expectedPath, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SaveGlobal_ConfigIsSerializedCorrectly()
        {
            // Arrange
            var config = new Config
            {
                IntroductionKey = "global_test_intro",
                ContextKey = "global_test_context"
            };

            var expectedPath = Path.Combine(_appDirectory, "config.json");
            string serializedJson = null;

            _mockFs.Setup(f => f.FileWriteAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .Callback<string, string>((path, content) => serializedJson = content)
                   .Returns(Task.CompletedTask);

            // Act
            await _configLoader.SaveGlobal(config);

            // Assert
            Assert.NotNull(serializedJson);
            Assert.Contains("global_test_intro", serializedJson);
            Assert.Contains("global_test_context", serializedJson);
        }

        #endregion

        #region Integration Tests (with real FileSystemWrapper)

        [Fact]
        public async Task LoadAndSave_Integration_ShouldWorkCorrectly()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var realFs = new FileSystemWrapper(_mockConstants.Object);
            var configLoader = new ConfigLoader(realFs, _mockConstants.Object);

            var originalConfig = new Config
            {
                IntroductionKey = "integration_test_intro",
                RulesKey = "integration_test_rules"
            };

            try
            {
                // Act - Save
                await configLoader.SaveGlobal(originalConfig);

                // Act - Load
                var loadedConfig = await configLoader.LoadGlobal();

                // Assert
                Assert.Equal(originalConfig.IntroductionKey, loadedConfig.IntroductionKey);
                Assert.Equal(originalConfig.RulesKey, loadedConfig.RulesKey);
            }
            finally
            {
                // Cleanup
                var configPath = Path.Combine(realFs.GetApplicationDirectory(), "config.json");
                if (realFs.FileExist(configPath))
                    realFs.FileDelete(configPath);
            }
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public async Task LoadGlobal_WhenFileReadThrowsException_PropagatesException()
        {
            // Arrange
            var expectedPath = Path.Combine(_appDirectory, "config.json");
            _mockFs.Setup(f => f.FileExist(expectedPath)).Returns(true);
            _mockFs.Setup(f => f.FileReadAsync(expectedPath))
                   .ThrowsAsync(new IOException("Test IO error"));
            _mockFs.Setup(f => f.PathCombine(_appDirectory, "config.json")).Returns(expectedPath);
            // Act & Assert
            await Assert.ThrowsAsync<IOException>(() => _configLoader.LoadGlobal());
        }

        [Fact]
        public async Task LoadLocal_PreservesDefaultValuesWhenPartialConfig()
        {
            // Arrange
            var expectedPath = Path.Combine(_testProjectDirectory, ".schiza", "config.json");
            var partialJson = @"
            {
                ""IntroductionKey"": ""partial_intro""
            }";

            _mockFs.Setup(f => f.FileExist(expectedPath)).Returns(true);
            _mockFs.Setup(f => f.FileReadAsync(expectedPath)).ReturnsAsync(partialJson);
            _mockFs.Setup(f => f.PathCombine(_testProjectDirectory, ".schiza", "config.json")).Returns(expectedPath);
            // Act
            var result = await _configLoader.LoadLocal(_testProjectDirectory);

            // Assert
            Assert.Equal("partial_intro", result.IntroductionKey);
            Assert.Equal("##context##", result.ContextKey); // Default preserved
            Assert.Equal("##rules##", result.RulesKey); // Default preserved
        }

        #endregion
    }
}