using Moq;
using Pdp.Inf.Interfaces;
using Pdp.Inf.Services;
namespace Pdp.Tests.InfrastructureTests
{
    public class FileSystemWrapperTests
    {
        [Fact]
        public void PathCombine_Test()
        {
            var str1 = "cat1";
            var str2 = "cat2";
            var str3 = "cat3";
            var fsc = new Mock<IFileSystemConstants>();
            var expected = Path.Combine(str1, str2, str3);
            var wrapper = new FileSystemWrapper(fsc.Object);

            var result1 = wrapper.PathCombine(str1, str2, str3);
            var result2 = wrapper.PathCombine([str1, str2, str3]);

            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }
    }
}
