using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Schiza.Tests.IntegrationTests
{
    public class DirectoryInfoWrapperIntegrationTests
    {
        [Fact]
        public void DirectoryInfoWrapper_Create_Should_Create_Wrapper_For_Existing_Directory()
        {
            // Arrange
            var testPath = Path.GetTempPath();

            // Act
            var wrapper = DirectoryInfoWrapper.CreateStatic(testPath);

            // Assert
            Assert.NotNull(wrapper);
            Assert.Equal(testPath.TrimEnd(Path.DirectorySeparatorChar), wrapper.FullName.TrimEnd(Path.DirectorySeparatorChar));
            Assert.True(wrapper.IsDirectory);
        }

        [Fact]
        public void DirectoryInfoWrapper_GetFileSystemInfos_Should_Return_Children()
        {
            // Arrange
            var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "test1.txt"), "content");
            File.WriteAllText(Path.Combine(testDir, "test2.txt"), "content");
            Directory.CreateDirectory(Path.Combine(testDir, "subdir"));

            try
            {
                var wrapper = DirectoryInfoWrapper.CreateStatic(testDir);

                // Act
                var children = wrapper.GetFileSystemInfos().ToList();

                // Assert
                Assert.Equal(3, children.Count());
                Assert.Contains(children, c => c.Name == "test1.txt" && !c.IsDirectory);
                Assert.Contains(children, c => c.Name == "test2.txt" && !c.IsDirectory);
                Assert.Contains(children, c => c.Name == "subdir" && c.IsDirectory);
            }
            finally
            {
                Directory.Delete(testDir, true);
            }
        }

        [Fact]
        public void DirectoryInfoWrapper_Create_For_File_Should_Not_Be_Directory()
        {
            // Arrange
            var testFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
            File.WriteAllText(testFile, "content");

            try
            {
                var wrapper = DirectoryInfoWrapper.CreateStatic(testFile);

                // Act & Assert
                Assert.NotNull(wrapper);
                Assert.Equal(testFile, wrapper.FullName);
                Assert.False(wrapper.IsDirectory);
            }
            finally
            {
                File.Delete(testFile);
            }
        }
    }
}
