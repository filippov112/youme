using Moq;
using Pdp.App.Interfaces;
using Pdp.App.Models;
using Pdp.App.Services;

namespace Pdp.Tests.Unit
{
    public class PromptBuilderTests
    {
        [Fact]
        public async Task GetPrompt_ShouldBuildPromptCorrectly()
        {
            // Arrange - настраиваем тестовые данные
            var config = new CombinationConfig
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "##intro##\n```\n##context##\n```\n##query##\n\n##rules##",
                FileStructure = "Файл: ##path##\n```\n##content##\n```",
                IntroductionText = "Ты - ассистент программиста",
                RulesText = "Отвечай на русском языке"
            };

            // Создаем моки
            var mockConfig = new Mock<IConfigService>();
            mockConfig
                .Setup(x => x.GetCombinationConfigAsync())
                .ReturnsAsync(config);

            var mockFs = new Mock<IFileSystemManager>();
            mockFs
                .Setup(x => x.ReadFileAsync("/project/Program.cs"))
                .ReturnsAsync("public class Program { static void Main() { Console.WriteLine(\"Hello\"); } }");
            mockFs
                .Setup(x => x.ReadFileAsync("/project/Utils.cs"))
                .ReturnsAsync("public static class Utils { public static void Helper() { } }");

            // Создаем тестируемый сервис
            var builder = new PromptBuilder(mockConfig.Object, mockFs.Object);

            // Act - вызываем метод сборки промпта
            var files = new List<string> { "/project/Program.cs", "/project/Utils.cs" };
            var query = "Что делает этот код?";
            var result = await builder.GetPrompt(files, query);

            // Assert - проверяем результат
            // 1. Проверяем наличие всех частей промпта
            Assert.Contains("Ты - ассистент программиста", result); // Вступление
            Assert.Contains("Что делает этот код?", result);         // Запрос
            Assert.Contains("Отвечай на русском языке", result);     // Правила

            // 2. Проверяем, что оба файла добавлены
            Assert.Contains("/project/Program.cs", result);
            Assert.Contains("/project/Utils.cs", result);

            // 3. Проверяем содержимое файлов
            Assert.Contains("public class Program { static void Main() { Console.WriteLine(\"Hello\"); } }", result);
            Assert.Contains("public static class Utils { public static void Helper() { } }", result);

            // 4. Проверяем структуру промпта
            Assert.Contains("```", result); // Проверяем наличие блоков кода
            Assert.Contains("Файл:", result); // Проверяем маркеры файлов

            // 5. Проверяем, что все ключи были заменены
            Assert.DoesNotContain("##intro##", result);
            Assert.DoesNotContain("##context##", result);
            Assert.DoesNotContain("##query##", result);
            Assert.DoesNotContain("##rules##", result);
            Assert.DoesNotContain("##path##", result);
            Assert.DoesNotContain("##content##", result);
        }
    }
}