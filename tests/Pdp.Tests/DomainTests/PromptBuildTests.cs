using Pdp.Domain.Models;
using Pdp.Domain.Models.PromptComponents;
using Pdp.Domain.Models.PromptComponents.FileComponents;
using File = Pdp.Domain.Models.PromptComponents.File;

namespace Pdp.Tests.DomainTests
{
    public class PromptBuildTests
    {
        [Fact]
        public void Build_ReturnCorrectlyPrompt()
        {
            var file1 = new File("""1. ##path##:##content##""", new FilePath("##path##", "/path1/filename.txt"), new FileContent("##content##", "***file-content1***"));
            var file2 = new File("""2. ##path##:##content##""", new FilePath("##path##", "/path2/filename.txt"), new FileContent("##content##", "***file-content2***"));
            var file3 = new File("""3. ##path##:##content##""", new FilePath("##path##", "/path3/filename.txt"), new FileContent("##content##", "***file-content3***"));
            var context = new Context("##project##", [file1, file2, file3]);
            var intro = new Introduction("##input##", "Введение:");
            var rules = new Rules("##settings##", "Правило1, правило2, правило3");
            var query = new Query("##request##", "Запрос1");
            var prompt = new Prompt("""Начало промпта. ##input##, '''##project##''', ##settings##. ##request##""", rules, query, intro, context);

            string result = prompt.Build();

            Assert.Equal("Начало промпта. Введение:, '''1. /path1/filename.txt:***file-content1***\n2. /path2/filename.txt:***file-content2***\n3. /path3/filename.txt:***file-content3***''', Правило1, правило2, правило3. Запрос1", result);
        }
    }
}
