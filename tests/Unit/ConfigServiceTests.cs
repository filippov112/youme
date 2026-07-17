using Core.Settings.Models;
using Core.Settings.Services;
using Moq;

namespace ProjectStudio.Test.Unit
{
    public class ConfigServiceTests
    {
        [Fact]
        public async Task GetCombinationConfigAsync_WithOnlyGlobalConfig_ShouldReturnGlobalConfig()
        {
            // Arrange
            var globalConfig = new Config
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "Global: ##intro##\n##context##\n##query##\n##rules##",
                FileStructure = "Global File: ##path##\n##content##",
                IntroductionDef = "Global Introduction",
                RulesDef = "Global Rules"
            };

            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync(globalConfig);
            mockLoader
                .Setup(x => x.LoadLocal(It.IsAny<string>()))
                .ReturnsAsync((Config?)null);

            var configService = new ConfigService(mockLoader.Object);
            await configService.SetRootDirectoryAsync(@"C:\Projects\MyApp");

            // Act
            var result = await configService.GetCombinationConfigAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("##intro##", result.IntroductionKey);
            Assert.Equal("##context##", result.ContextKey);
            Assert.Equal("##rules##", result.RulesKey);
            Assert.Equal("##query##", result.QueryKey);
            Assert.Equal("##path##", result.FilePathKey);
            Assert.Equal("##content##", result.FileContentKey);
            Assert.Equal("Global: ##intro##\n##context##\n##query##\n##rules##", result.PromptStructure);
            Assert.Equal("Global File: ##path##\n##content##", result.FileStructure);
            Assert.Equal("Global Introduction", result.IntroductionText);
            Assert.Equal("Global Rules", result.RulesText);
        }

        [Fact]
        public async Task GetCombinationConfigAsync_WithLocalConfig_ShouldOverrideGlobal()
        {
            // Arrange
            var globalConfig = new Config
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "Global: ##intro##\n##context##\n##query##\n##rules##",
                FileStructure = "Global File: ##path##\n##content##",
                IntroductionDef = "Global Introduction",
                RulesDef = "Global Rules"
            };

            var localConfig = new Config
            {
                IntroductionKey = "{{intro}}",
                ContextKey = "{{context}}",
                RulesKey = "{{rules}}",
                QueryKey = "{{query}}",
                FilePathKey = "{{path}}",
                FileContentKey = "{{content}}",
                PromptStructure = "Local: {{intro}}\n{{context}}\n{{query}}\n{{rules}}",
                FileStructure = "Local File: {{path}}\n{{content}}",
                IntroductionDef = "Local Introduction",
                RulesDef = "Local Rules"
            };

            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync(globalConfig);
            mockLoader
                .Setup(x => x.LoadLocal(It.IsAny<string>()))
                .ReturnsAsync(localConfig);

            var configService = new ConfigService(mockLoader.Object);
            await configService.SetRootDirectoryAsync(@"C:\Projects\MyApp");

            // Act
            var result = await configService.GetCombinationConfigAsync();

            // Assert - локальная конфигурация должна переопределить глобальную
            Assert.NotNull(result);
            Assert.Equal("{{intro}}", result.IntroductionKey);
            Assert.Equal("{{context}}", result.ContextKey);
            Assert.Equal("{{rules}}", result.RulesKey);
            Assert.Equal("{{query}}", result.QueryKey);
            Assert.Equal("{{path}}", result.FilePathKey);
            Assert.Equal("{{content}}", result.FileContentKey);
            Assert.Equal("Local: {{intro}}\n{{context}}\n{{query}}\n{{rules}}", result.PromptStructure);
            Assert.Equal("Local File: {{path}}\n{{content}}", result.FileStructure);
            Assert.Equal("Local Introduction", result.IntroductionText);
            Assert.Equal("Local Rules", result.RulesText);
        }

        [Fact]
        public async Task GetCombinationConfigAsync_WithLocalOverridePartial_ShouldMergeConfigs()
        {
            // Arrange
            var globalConfig = new Config
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "Global Structure",
                FileStructure = "Global File Structure",
                IntroductionDef = "Global Introduction",
                RulesDef = "Global Rules"
            };

            var localConfig = new Config
            {
                IntroductionKey = "{{intro}}", // Переопределяем только вступление
                ContextKey = "##context##",    // Оставляем как в глобальном
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "Local Structure", // Переопределяем структуру
                FileStructure = "Global File Structure", // Оставляем как в глобальном
                IntroductionDef = "Local Introduction",
                RulesDef = "Global Rules"
            };

            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync(globalConfig);
            mockLoader
                .Setup(x => x.LoadLocal(It.IsAny<string>()))
                .ReturnsAsync(localConfig);

            var configService = new ConfigService(mockLoader.Object);
            await configService.SetRootDirectoryAsync(@"C:\Projects\MyApp");

            // Act
            var result = await configService.GetCombinationConfigAsync();

            // Assert - проверяем смешанную конфигурацию
            Assert.NotNull(result);
            Assert.Equal("{{intro}}", result.IntroductionKey); // Из локальной
            Assert.Equal("##context##", result.ContextKey);    // Из глобальной
            Assert.Equal("##rules##", result.RulesKey);        // Из глобальной
            Assert.Equal("##query##", result.QueryKey);        // Из глобальной
            Assert.Equal("##path##", result.FilePathKey);      // Из глобальной
            Assert.Equal("##content##", result.FileContentKey);// Из глобальной
            Assert.Equal("Local Structure", result.PromptStructure); // Из локальной
            Assert.Equal("Global File Structure", result.FileStructure); // Из глобальной
            Assert.Equal("Local Introduction", result.IntroductionText); // Из локальной
            Assert.Equal("Global Rules", result.RulesText); // Из глобальной
        }

        [Fact]
        public async Task GetAllConfigAsync_ShouldReturnBothConfigs()
        {
            // Arrange
            var globalConfig = new Config
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "Global Structure",
                FileStructure = "Global File Structure",
                IntroductionDef = "Global Introduction",
                RulesDef = "Global Rules"
            };

            var localConfig = new Config
            {
                IntroductionKey = "{{intro}}",
                ContextKey = "{{context}}",
                RulesKey = "{{rules}}",
                QueryKey = "{{query}}",
                FilePathKey = "{{path}}",
                FileContentKey = "{{content}}",
                PromptStructure = "Local Structure",
                FileStructure = "Local File Structure",
                IntroductionDef = "Local Introduction",
                RulesDef = "Local Rules"
            };

            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync(globalConfig);
            mockLoader
                .Setup(x => x.LoadLocal(It.IsAny<string>()))
                .ReturnsAsync(localConfig);

            var configService = new ConfigService(mockLoader.Object);
            await configService.SetRootDirectoryAsync(@"C:\Projects\MyApp");

            // Act
            var result = await configService.GetAllConfigAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Global);
            Assert.NotNull(result.Local);

            // Проверяем глобальную конфигурацию
            Assert.Equal("##intro##", result.Global.IntroductionKey);
            Assert.Equal("Global Introduction", result.Global.IntroductionText);
            Assert.Equal("Global Rules", result.Global.RulesText);
            Assert.Equal("Global Structure", result.Global.PromptStructure);

            // Проверяем локальную конфигурацию
            Assert.Equal("{{intro}}", result.Local.IntroductionKey);
            Assert.Equal("Local Introduction", result.Local.IntroductionText);
            Assert.Equal("Local Rules", result.Local.RulesText);
            Assert.Equal("Local Structure", result.Local.PromptStructure);
        }

        [Fact]
        public async Task SaveAllConfigAsync_ShouldSaveBothConfigs()
        {
            // Arrange
            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync(new Config());

            var configService = new ConfigService(mockLoader.Object);
            await configService.SetRootDirectoryAsync(@"C:\Projects\MyApp");

            var allConfig = new AllConfigDto
            {
                Global = new CombinationConfig
                {
                    IntroductionKey = "##intro##",
                    IntroductionText = "Updated Global Intro",
                    RulesText = "Updated Global Rules",
                    PromptStructure = "Updated Global Structure"
                },
                Local = new CombinationConfig
                {
                    IntroductionKey = "{{intro}}",
                    IntroductionText = "Updated Local Intro",
                    RulesText = "Updated Local Rules",
                    PromptStructure = "Updated Local Structure"
                }
            };

            // Act
            await configService.SaveAllConfigAsync(allConfig);

            // Assert
            mockLoader.Verify(
                x => x.SaveGlobal(It.Is<Config>(c =>
                    c.IntroductionDef == "Updated Global Intro" &&
                    c.RulesDef == "Updated Global Rules" &&
                    c.PromptStructure == "Updated Global Structure"
                )),
                Times.Once
            );

            mockLoader.Verify(
                x => x.SaveLocal(It.Is<Config>(c =>
                    c.IntroductionDef == "Updated Local Intro" &&
                    c.RulesDef == "Updated Local Rules" &&
                    c.PromptStructure == "Updated Local Structure"
                ), @"C:\Projects\MyApp"),
                Times.Once
            );
        }

        [Fact]
        public async Task SetRootDirectoryAsync_ShouldLoadLocalConfig()
        {
            // Arrange
            var globalConfig = new Config
            {
                IntroductionDef = "Global Intro",
                RulesDef = "Global Rules"
            };

            var localConfig = new Config
            {
                IntroductionDef = "Local Intro",
                RulesDef = "Local Rules"
            };

            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync(globalConfig);
            mockLoader
                .Setup(x => x.LoadLocal(@"C:\Projects\MyApp"))
                .ReturnsAsync(localConfig);

            var configService = new ConfigService(mockLoader.Object);

            // Act
            await configService.SetRootDirectoryAsync(@"C:\Projects\MyApp");
            var result = await configService.GetCombinationConfigAsync();

            // Assert
            Assert.Equal(@"C:\Projects\MyApp", configService.RootDirectory);
            Assert.Equal("Local Intro", result.IntroductionText);
            Assert.Equal("Local Rules", result.RulesText);

            mockLoader.Verify(x => x.LoadLocal(@"C:\Projects\MyApp"), Times.Once);
        }

        [Fact]
        public async Task GetCombinationConfigAsync_WithNoConfigs_ShouldReturnDefaultConfig()
        {
            // Arrange
            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync((Config?)null);
            mockLoader
                .Setup(x => x.LoadLocal(It.IsAny<string>()))
                .ReturnsAsync((Config?)null);

            var configService = new ConfigService(mockLoader.Object);
            await configService.SetRootDirectoryAsync(@"C:\Projects\MyApp");

            // Act
            var result = await configService.GetCombinationConfigAsync();

            // Assert - должна вернуться конфигурация по умолчанию
            Assert.NotNull(result);
            Assert.Equal("##intro##", result.IntroductionKey);
            Assert.Equal("##context##", result.ContextKey);
            Assert.Equal("##rules##", result.RulesKey);
            Assert.Equal("##query##", result.QueryKey);
            Assert.Equal("##path##", result.FilePathKey);
            Assert.Equal("##content##", result.FileContentKey);
            Assert.Contains("##intro##", result.PromptStructure);
            Assert.Contains("##context##", result.PromptStructure);
            Assert.Contains("##query##", result.PromptStructure);
            Assert.Contains("##rules##", result.PromptStructure);
            Assert.Empty(result.IntroductionText);
            Assert.Empty(result.RulesText);
        }

        [Fact]
        public async Task GetCombinationConfigAsync_WithMultipleSetRoot_ShouldReloadLocalConfig()
        {
            // Arrange
            var globalConfig = new Config { IntroductionDef = "Global Intro" };

            var localConfig1 = new Config { IntroductionDef = "Project1 Intro" };
            var localConfig2 = new Config { IntroductionDef = "Project2 Intro" };

            var mockLoader = new Mock<IConfigLoader>();
            mockLoader
                .Setup(x => x.LoadGlobal())
                .ReturnsAsync(globalConfig);
            mockLoader
                .Setup(x => x.LoadLocal(@"C:\Projects\Project1"))
                .ReturnsAsync(localConfig1);
            mockLoader
                .Setup(x => x.LoadLocal(@"C:\Projects\Project2"))
                .ReturnsAsync(localConfig2);

            var configService = new ConfigService(mockLoader.Object);

            // Act - первый проект
            await configService.SetRootDirectoryAsync(@"C:\Projects\Project1");
            var result1 = await configService.GetCombinationConfigAsync();

            // Assert - первый проект
            Assert.Equal("Project1 Intro", result1.IntroductionText);

            // Act - второй проект
            await configService.SetRootDirectoryAsync(@"C:\Projects\Project2");
            var result2 = await configService.GetCombinationConfigAsync();

            // Assert - второй проект
            Assert.Equal("Project2 Intro", result2.IntroductionText);
        }

        [Fact]
        public void GetAllConfigDefault_ShouldReturnDefaultConfigs()
        {
            // Arrange
            var mockLoader = new Mock<IConfigLoader>();
            var configService = new ConfigService(mockLoader.Object);

            // Act
            var result = configService.GetAllConfigDefault();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Global);
            Assert.NotNull(result.Local);

            // Проверяем, что обе конфигурации имеют значения по умолчанию
            Assert.Equal("##intro##", result.Global.IntroductionKey);
            Assert.Equal("##context##", result.Global.ContextKey);
            Assert.Equal("##rules##", result.Global.RulesKey);
            Assert.Equal("##query##", result.Global.QueryKey);

            Assert.Equal("##intro##", result.Local.IntroductionKey);
            Assert.Equal("##context##", result.Local.ContextKey);
            Assert.Equal("##rules##", result.Local.RulesKey);
            Assert.Equal("##query##", result.Local.QueryKey);
        }
    }
}