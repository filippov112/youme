using Application.Interfaces;
using Application.Models;
using Application.Services;
using Moq;

namespace Schiza.Tests.ApplicationTests
{
    public class ConfigServiceTests
    {
        private readonly Mock<IConfigLoader> _mockLoader;
        private readonly ConfigService _configService;

        public ConfigServiceTests()
        {
            _mockLoader = new Mock<IConfigLoader>();
            var mock_observer = new Mock<ICatalogObserver>();
            _configService = new ConfigService(_mockLoader.Object, mock_observer.Object);
        }

        [Fact]
        public async Task Constructor_ShouldCallLoadGlobal()
        {
            var mock_observer = new Mock<ICatalogObserver>();
            // Arrange
            var expectedGlobalConfig = new Config
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##"
            };
            var mock = new Mock<IConfigLoader>();
            mock.Setup(x => x.LoadGlobal()).ReturnsAsync(expectedGlobalConfig);

            // Act
            var service = new ConfigService(mock.Object, mock_observer.Object);

            // Assert
            var result = service.GetCombinationConfigAsync();
            Assert.NotNull(result);
            mock.Verify(x => x.LoadGlobal(), Times.Once);
        }

        [Fact]
        public async Task SetRootDirectory_ShouldLoadLocalConfig()
        {
            // Arrange
            string rootDirectory = "C:\\test\\project";
            var expectedLocalConfig = new Config
            {
                IntroductionKey = "local_intro",
                RulesDef = "local_rules"
            };

            _mockLoader.Setup(x => x.LoadLocal(rootDirectory)).ReturnsAsync(expectedLocalConfig);

            // Act
            await _configService.SetRootDirectoryAsync(rootDirectory);

            // Assert
            Assert.Equal(rootDirectory, _configService.RootDirectory);
            _mockLoader.Verify(x => x.LoadLocal(rootDirectory), Times.Once);
        }

        [Fact]
        public async Task SetRootDirectory_ShouldUpdateLocalConfig()
        {
            // Arrange
            string rootDirectory1 = "C:\\test\\project1";
            string rootDirectory2 = "C:\\test\\project2";
            var localConfig1 = new Config { IntroductionKey = "config1" };
            var localConfig2 = new Config { IntroductionKey = "config2" };

            _mockLoader.Setup(x => x.LoadLocal(rootDirectory1)).ReturnsAsync(localConfig1);
            _mockLoader.Setup(x => x.LoadLocal(rootDirectory2)).ReturnsAsync(localConfig2);

            // Act
            await _configService.SetRootDirectoryAsync(rootDirectory1);
            var result1 = await _configService.GetCombinationConfigAsync();

            await _configService.SetRootDirectoryAsync(rootDirectory2);
            var result2 = await _configService.GetCombinationConfigAsync();

            // Assert
            Assert.Equal(rootDirectory2, _configService.RootDirectory);
            Assert.Equal("config1", result1.IntroductionKey);
            Assert.Equal("config2", result2.IntroductionKey);
        }

        [Fact]
        public async Task GetCombinationConfig_WhenLocalConfigExists_ShouldReturnLocalConfig()
        {
            // Arrange
            var globalConfig = new Config { IntroductionKey = "global_intro", RulesDef = "global_rules" };
            var localConfig = new Config { IntroductionKey = "local_intro", RulesDef = "local_rules" };

            _mockLoader.Setup(x => x.LoadGlobal()).ReturnsAsync(globalConfig);
            _mockLoader.Setup(x => x.LoadLocal(It.IsAny<string>())).ReturnsAsync(localConfig);

            await _configService.SetRootDirectoryAsync("C:\\test");

            // Act
            var result = await _configService.GetCombinationConfigAsync();

            // Assert
            Assert.Equal("local_intro", result.IntroductionKey);
            Assert.Equal("local_rules", result.RulesText);
        }

        [Fact]
        public async Task GetCombinationConfig_WhenLocalConfigNotExists_ShouldReturnGlobalConfig()
        {
            // Arrange
            var globalConfig = new Config { IntroductionKey = "global_intro", RulesDef = "global_rules" };
            var mock = new Mock<IConfigLoader>();
            var mock_observer = new Mock<ICatalogObserver>();
            mock.Setup(x => x.LoadGlobal()).ReturnsAsync(globalConfig);
            mock.Setup(x => x.LoadLocal(It.IsAny<string>())).ReturnsAsync((Config?)null);
            var configService = new ConfigService(mock.Object, mock_observer.Object);

            await configService.SetRootDirectoryAsync("C:\\test");


            // Act
            var result = await configService.GetCombinationConfigAsync();

            // Assert
            Assert.Equal("global_intro", result.IntroductionKey);
            Assert.Equal("global_rules", result.RulesText);
        }

        [Fact]
        public async Task GetCombinationConfig_WhenNoConfigsExist_ShouldReturnDefaultConfig()
        {
            var mock_observer = new Mock<ICatalogObserver>();
            // Arrange
            _mockLoader.Setup(x => x.LoadGlobal()).ReturnsAsync(new Config());
            _mockLoader.Setup(x => x.LoadLocal(It.IsAny<string>())).ReturnsAsync((Config?)null);
            var configService = new ConfigService(_mockLoader.Object, mock_observer.Object);
            // Act
            var result = await configService.GetCombinationConfigAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("##intro##", result.IntroductionKey);
            Assert.Equal("##context##", result.ContextKey);
            Assert.Equal("##rules##", result.RulesKey);
        }

        [Fact]
        public async Task GetAllConfig_ShouldReturnCorrectAllConfigDto()
        {
            // Arrange
            var globalConfig = new Config
            {
                IntroductionKey = "global_intro",
                RulesDef = "global_rules",
                PromptStructure = "global_prompt"
            };

            var localConfig = new Config
            {
                IntroductionKey = "local_intro",
                RulesDef = "local_rules",
                PromptStructure = "local_prompt"
            };
            var mock_observer = new Mock<ICatalogObserver>();
            _mockLoader.Setup(x => x.LoadGlobal()).ReturnsAsync(globalConfig);
            _mockLoader.Setup(x => x.LoadLocal(It.IsAny<string>())).ReturnsAsync(localConfig);
            var configService = new ConfigService(_mockLoader.Object, mock_observer.Object);
            await configService.SetRootDirectoryAsync("C:\\test");

            // Act
            var result = await configService.GetAllConfigAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Global);
            Assert.NotNull(result.Local);
            Assert.Equal("global_intro", result.Global.IntroductionKey);
            Assert.Equal("local_intro", result.Local.IntroductionKey);
        }

        [Fact]
        public async Task GetAllConfig_WhenLocalConfigIsNull_ShouldReturnNullLocal()
        {
            // Arrange
            var globalConfig = new Config { IntroductionKey = "global_intro" };
            var mock_observer = new Mock<ICatalogObserver>();
            _mockLoader.Setup(x => x.LoadGlobal()).ReturnsAsync(globalConfig);
            _mockLoader.Setup(x => x.LoadLocal(It.IsAny<string>())).ReturnsAsync((Config?)null);
            var configService = new ConfigService(_mockLoader.Object, mock_observer.Object);
            await configService.SetRootDirectoryAsync("C:\\test");

            // Act
            var result = await configService.GetAllConfigAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Global);
            Assert.Null(result.Local);
        }

        [Fact]
        public async Task SaveAllConfig_ShouldSaveAndUpdateGlobalConfig()
        {
            // Arrange
            var allConfigDto = new AllConfigDto
            {
                Global = new CombinationConfig { IntroductionKey = "new_global_intro", RulesText = "new_global_rules" },
                Local = null
            };

            // Act
            await _configService.SaveAllConfigAsync(allConfigDto);

            // Assert
            _mockLoader.Verify(x => x.SaveGlobal(It.Is<Config>(c =>
                c.IntroductionKey == "new_global_intro" &&
                c.RulesDef == "new_global_rules")), Times.Once);

            _mockLoader.Verify(x => x.SaveLocal(It.IsAny<Config>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SaveAllConfig_WithLocalConfig_ShouldSaveBothConfigs()
        {
            // Arrange
            string rootDirectory = "C:\\test";
            var allConfigDto = new AllConfigDto
            {
                Global = new CombinationConfig { IntroductionKey = "new_global_intro", RulesText = "new_global_rules" },
                Local = new CombinationConfig { IntroductionKey = "new_local_intro", RulesText = "new_local_rules" }
            };

            await _configService.SetRootDirectoryAsync(rootDirectory);

            // Act
            await _configService.SaveAllConfigAsync(allConfigDto);

            // Assert
            _mockLoader.Verify(x => x.SaveGlobal(It.Is<Config>(c =>
                c.IntroductionKey == "new_global_intro")), Times.Once);

            _mockLoader.Verify(x => x.SaveLocal(It.Is<Config>(c =>
                c.IntroductionKey == "new_local_intro"), rootDirectory), Times.Once);
        }

        [Fact]
        public async Task SaveAllConfig_ShouldUpdateInternalConfigs()
        {
            // Arrange
            var allConfigDto = new AllConfigDto
            {
                Global = new CombinationConfig { IntroductionKey = "updated_global", RulesText = "updated_rules" },
                Local = new CombinationConfig { IntroductionKey = "updated_local" }
            };

            await _configService.SetRootDirectoryAsync("C:\\test");

            // Act
            await _configService.SaveAllConfigAsync(allConfigDto);

            // Проверяем, что GetCombinationConfig возвращает обновленную конфигурацию
            var result = await _configService.GetCombinationConfigAsync();

            // Assert
            Assert.Equal("updated_local", result.IntroductionKey);
        }

        [Fact]
        public async Task GetCombinationConfig_ShouldCombineCorrectlyFromLocalAndGlobal()
        {
            // Arrange
            var globalConfig = new Config
            {
                IntroductionKey = "global_intro",
                ContextKey = "global_context",
                RulesKey = "global_rules",
                QueryKey = "global_query",
                FilePathKey = "global_path",
                FileContentKey = "global_content",
                PromptStructure = "global_prompt",
                FileStructure = "global_file",
                IntroductionDef = "global_intro_def",
                RulesDef = "global_rules_def"
            };

            var localConfig = new Config
            {
                IntroductionKey = "local_intro", // Только это переопределено локально
                ContextKey = "global_context", // Остальное из глобальной
                RulesKey = "global_rules",
                QueryKey = "global_query",
                FilePathKey = "global_path",
                FileContentKey = "global_content",
                PromptStructure = "global_prompt",
                FileStructure = "global_file",
                IntroductionDef = "global_intro_def",
                RulesDef = "global_rules_def"
            };

            _mockLoader.Setup(x => x.LoadGlobal()).ReturnsAsync(globalConfig);
            _mockLoader.Setup(x => x.LoadLocal(It.IsAny<string>())).ReturnsAsync(localConfig);

            await _configService.SetRootDirectoryAsync("C:\\test");

            // Act
            var result = await _configService.GetCombinationConfigAsync();

            // Assert
            Assert.Equal("local_intro", result.IntroductionKey);
            Assert.Equal("global_context", result.ContextKey);
            Assert.Equal("global_rules", result.RulesKey);
            Assert.Equal("global_query", result.QueryKey);
            Assert.Equal("global_path", result.FilePathKey);
            Assert.Equal("global_content", result.FileContentKey);
            Assert.Equal("global_prompt", result.PromptStructure);
            Assert.Equal("global_file", result.FileStructure);
        }
    }
}