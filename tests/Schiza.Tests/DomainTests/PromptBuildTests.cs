using Domain.Models;
using Domain.Models.PromptComponents;
using Domain.Models.PromptComponents.FileComponents;
using File = Domain.Models.PromptComponents.File;

namespace Schiza.Tests.DomainTests
{
    public class PromptBuildTests
    {
        [Fact]
        public void Test()
        {
            var file1 = new File("""1. ##path##:##content##""", new FilePath("/path1/filename.txt"), new FileContent("***file-content1***"));
            var file2 = new File("""2. ##path##:##content##""", new FilePath("/path2/filename.txt"), new FileContent("***file-content2***"));
            var file3 = new File("""3. ##path##:##content##""", new FilePath("/path3/filename.txt"), new FileContent("***file-content3***"));
            var context = new Context([file1, file2, file3]);
            var intro = new Introduction("Введение:");
            var rules = new Rules("Правило1, правило2, правило3");
            var query = new Query("Запрос1");
            var prompt = new Prompt("""Начало промпта. ##input##, '''##project##''', ##settings##. ##request##""", rules, query, intro, context);

            string result = prompt.Build();

            Assert.Equal("Начало промпта. Введение:, '''1. /path1/filename.txt:***file-content1***\n2. /path2/filename.txt:***file-content2***\n3. /path3/filename.txt:***file-content3***''', Правило1, правило2, правило3. Запрос1", result);
        }
    }
}
