using Infrastructure.Interfaces;
using Infrastructure.Services;
using Moq;

namespace Schiza.Tests.InfrastructureTests
{
    public class FileSystemManagerTests
    {
        private readonly Mock<IFileSystemWrapper> _mockFileSystemWrapper;
        private readonly Mock<IDirectoryInfoWrapper> _mockDirectoryInfoFactory;
        private readonly FileSystemManager _fileSystemManager;

        public FileSystemManagerTests()
        {
            _mockFileSystemWrapper = new Mock<IFileSystemWrapper>();
            _mockDirectoryInfoFactory = new Mock<IDirectoryInfoWrapper>();
            _fileSystemManager = new FileSystemManager(
                _mockFileSystemWrapper.Object,
                _mockDirectoryInfoFactory.Object);
        }

        #region FileExistsAsync Tests

        [Fact]
        public async Task FileExistsAsync_WhenFileExists_ReturnsTrue()
        {
            // Arrange
            string path = "C:\\test\\file.txt";
            _mockFileSystemWrapper.Setup(x => x.FileExist(path)).Returns(true);

            // Act
            var result = await _fileSystemManager.FileExistsAsync(path);

            // Assert
            Assert.True(result);
            _mockFileSystemWrapper.Verify(x => x.FileExist(path), Times.Once);
        }

        [Fact]
        public async Task FileExistsAsync_WhenFileDoesNotExist_ReturnsFalse()
        {
            // Arrange
            string path = "C:\\test\\file.txt";
            _mockFileSystemWrapper.Setup(x => x.FileExist(path)).Returns(false);

            // Act
            var result = await _fileSystemManager.FileExistsAsync(path);

            // Assert
            Assert.False(result);
            _mockFileSystemWrapper.Verify(x => x.FileExist(path), Times.Once);
        }

        #endregion

        #region DirectoryExistsAsync Tests

        [Fact]
        public async Task DirectoryExistsAsync_WhenDirectoryExists_ReturnsTrue()
        {
            // Arrange
            string path = "C:\\test";
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(path)).Returns(true);

            // Act
            var result = await _fileSystemManager.DirectoryExistsAsync(path);

            // Assert
            Assert.True(result);
            _mockFileSystemWrapper.Verify(x => x.DirectoryExist(path), Times.Once);
        }

        [Fact]
        public async Task DirectoryExistsAsync_WhenDirectoryDoesNotExist_ReturnsFalse()
        {
            // Arrange
            string path = "C:\\test";
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(path)).Returns(false);

            // Act
            var result = await _fileSystemManager.DirectoryExistsAsync(path);

            // Assert
            Assert.False(result);
            _mockFileSystemWrapper.Verify(x => x.DirectoryExist(path), Times.Once);
        }

        #endregion

        #region ReadFileAsync Tests

        [Fact]
        public async Task ReadFileAsync_WhenFileExists_ReturnsFileContent()
        {
            // Arrange
            string path = "C:\\test\\file.txt";
            string expectedContent = "Hello, World!";
            _mockFileSystemWrapper.Setup(x => x.FileReadAsync(path)).ReturnsAsync(expectedContent);

            // Act
            var result = await _fileSystemManager.ReadFileAsync(path);

            // Assert
            Assert.Equal(expectedContent, result);
            _mockFileSystemWrapper.Verify(x => x.FileReadAsync(path), Times.Once);
        }

        [Fact]
        public async Task ReadFileAsync_WhenFileDoesNotExist_ThrowsException()
        {
            // Arrange
            string path = "C:\\test\\file.txt";
            _mockFileSystemWrapper.Setup(x => x.FileReadAsync(path))
                .ThrowsAsync(new FileNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _fileSystemManager.ReadFileAsync(path));
        }

        #endregion

        #region WriteFileAsync Tests

        [Fact]
        public async Task WriteFileAsync_WhenDirectoryExists_WritesFileDirectly()
        {
            // Arrange
            string path = "C:\\test\\file.txt";
            string content = "Test content";
            string directory = "C:\\test";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(path)).Returns(directory);
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(directory)).Returns(true);

            // Act
            await _fileSystemManager.WriteFileAsync(path, content);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.CreateDirectory(directory), Times.Never);
            _mockFileSystemWrapper.Verify(x => x.FileWriteAsync(path, content), Times.Once);
        }

        [Fact]
        public async Task WriteFileAsync_WhenDirectoryDoesNotExist_CreatesDirectoryBeforeWriting()
        {
            // Arrange
            string path = "C:\\test\\sub\\file.txt";
            string content = "Test content";
            string directory = "C:\\test\\sub";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(path)).Returns(directory);
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(directory)).Returns(false);

            // Act
            await _fileSystemManager.WriteFileAsync(path, content);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.CreateDirectory(directory), Times.Once);
            _mockFileSystemWrapper.Verify(x => x.FileWriteAsync(path, content), Times.Once);
        }

        [Fact]
        public async Task WriteFileAsync_WhenPathHasNoDirectory_WritesFileDirectly()
        {
            // Arrange
            string path = "file.txt";
            string content = "Test content";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(path)).Returns((string?)null);

            // Act
            await _fileSystemManager.WriteFileAsync(path, content);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
            _mockFileSystemWrapper.Verify(x => x.FileWriteAsync(path, content), Times.Once);
        }

        #endregion

        #region GetTreeAsync Tests

        [Fact]
        public async Task GetTreeAsync_WhenRootIsDirectory_ReturnsCompleteTree()
        {
            // Arrange
            string rootPath = "C:\\project";
            var mockRootInfo = new Mock<IDirectoryInfoWrapper>();
            var mockSubDirInfo = new Mock<IDirectoryInfoWrapper>();
            var mockFileInfo = new Mock<IDirectoryInfoWrapper>();
            var mockUnreadableFileInfo = new Mock<IDirectoryInfoWrapper>();

            mockRootInfo.Setup(x => x.FullName).Returns("C:\\project");
            mockRootInfo.Setup(x => x.Name).Returns("project");
            mockRootInfo.Setup(x => x.IsDirectory).Returns(true);

            mockSubDirInfo.Setup(x => x.FullName).Returns("C:\\project\\subdir");
            mockSubDirInfo.Setup(x => x.Name).Returns("subdir");
            mockSubDirInfo.Setup(x => x.IsDirectory).Returns(true);
            mockSubDirInfo.Setup(x => x.GetFileSystemInfos()).Returns(new List<IDirectoryInfoWrapper>());

            mockFileInfo.Setup(x => x.FullName).Returns("C:\\project\\readable.txt");
            mockFileInfo.Setup(x => x.Name).Returns("readable.txt");
            mockFileInfo.Setup(x => x.IsDirectory).Returns(false);

            mockUnreadableFileInfo.Setup(x => x.FullName).Returns("C:\\project\\unreadable.txt");
            mockUnreadableFileInfo.Setup(x => x.Name).Returns("unreadable.txt");
            mockUnreadableFileInfo.Setup(x => x.IsDirectory).Returns(false);

            mockRootInfo.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper>
                {
                    mockSubDirInfo.Object,
                    mockFileInfo.Object,
                    mockUnreadableFileInfo.Object
                });

            _mockDirectoryInfoFactory.Setup(x => x.Create(rootPath)).Returns(mockRootInfo.Object);

            // Setup readability checks
            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(It.IsAny<string>())).Returns(true);
            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockUnreadableFileInfo.Object.FullName)).Returns(false);

            // Act
            var result = await _fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("project", result.Name);
            Assert.Equal("C:\\project", result.Path);
            Assert.True(result.IsDirectory);
            Assert.Null(result.Parent);
            Assert.Equal(2, result.Children.Count); // Unreadable file should be skipped

            Assert.Contains(result.Children, c => c.Name == "subdir");
            Assert.Contains(result.Children, c => c.Name == "readable.txt");
            Assert.DoesNotContain(result.Children, c => c.Name == "unreadable.txt");
        }

        [Fact]
        public async Task GetTreeAsync_WhenRootIsReadableFile_ReturnsFileNode()
        {
            // Arrange
            string rootPath = "C:\\project\\file.txt";
            var mockFileInfo = new Mock<IDirectoryInfoWrapper>();

            mockFileInfo.Setup(x => x.FullName).Returns("C:\\project\\file.txt");
            mockFileInfo.Setup(x => x.Name).Returns("file.txt");
            mockFileInfo.Setup(x => x.IsDirectory).Returns(false);

            _mockDirectoryInfoFactory.Setup(x => x.Create(rootPath)).Returns(mockFileInfo.Object);
            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockFileInfo.Object.FullName)).Returns(true);

            // Act
            var result = await _fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("file.txt", result.Name);
            Assert.Equal("C:\\project\\file.txt", result.Path);
            Assert.False(result.IsDirectory);
            Assert.Empty(result.Children);
            _mockFileSystemWrapper.Verify(x => x.FileIsReadable(mockFileInfo.Object.FullName), Times.Once);
        }

        [Fact]
        public async Task GetTreeAsync_WhenRootIsUnreadableFile_ReturnsNull()
        {
            // Arrange
            string rootPath = "C:\\project\\unreadable.txt";
            var mockFileInfo = new Mock<IDirectoryInfoWrapper>();

            mockFileInfo.Setup(x => x.FullName).Returns("C:\\project\\unreadable.txt");
            mockFileInfo.Setup(x => x.Name).Returns("unreadable.txt");
            mockFileInfo.Setup(x => x.IsDirectory).Returns(false);

            _mockDirectoryInfoFactory.Setup(x => x.Create(rootPath)).Returns(mockFileInfo.Object);
            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockFileInfo.Object.FullName)).Returns(false);

            // Act
            var result = await _fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.Null(result);
            _mockFileSystemWrapper.Verify(x => x.FileIsReadable(mockFileInfo.Object.FullName), Times.Once);
        }

        [Fact]
        public async Task GetTreeAsync_WhenDirectoryContainsUnreadableSubdirectories_ProcessesOnlyReadableOnes()
        {
            // Arrange
            string rootPath = "C:\\project";
            var mockRootInfo = new Mock<IDirectoryInfoWrapper>();
            var mockReadableSubDir = new Mock<IDirectoryInfoWrapper>();
            var mockUnreadableSubDir = new Mock<IDirectoryInfoWrapper>();
            var mockFileInUnreadableDir = new Mock<IDirectoryInfoWrapper>();

            mockRootInfo.Setup(x => x.FullName).Returns("C:\\project");
            mockRootInfo.Setup(x => x.Name).Returns("project");
            mockRootInfo.Setup(x => x.IsDirectory).Returns(true);

            // Readable subdirectory
            mockReadableSubDir.Setup(x => x.FullName).Returns("C:\\project\\readable");
            mockReadableSubDir.Setup(x => x.Name).Returns("readable");
            mockReadableSubDir.Setup(x => x.IsDirectory).Returns(true);
            mockReadableSubDir.Setup(x => x.GetFileSystemInfos()).Returns(new List<IDirectoryInfoWrapper>());

            // Unreadable subdirectory (but still accessible as directory)
            mockUnreadableSubDir.Setup(x => x.FullName).Returns("C:\\project\\unreadable");
            mockUnreadableSubDir.Setup(x => x.Name).Returns("unreadable");
            mockUnreadableSubDir.Setup(x => x.IsDirectory).Returns(true);

            // File inside unreadable directory (should not be processed because parent is directory, not file)
            mockFileInUnreadableDir.Setup(x => x.FullName).Returns("C:\\project\\unreadable\\file.txt");
            mockFileInUnreadableDir.Setup(x => x.Name).Returns("file.txt");
            mockFileInUnreadableDir.Setup(x => x.IsDirectory).Returns(false);

            mockUnreadableSubDir.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper> { mockFileInUnreadableDir.Object });

            mockRootInfo.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper> { mockReadableSubDir.Object, mockUnreadableSubDir.Object });

            _mockDirectoryInfoFactory.Setup(x => x.Create(rootPath)).Returns(mockRootInfo.Object);

            // Only files in readable subdirectory are checked for readability
            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(It.IsAny<string>())).Returns(true);

            // Act
            var result = await _fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Children.Count);

            var readableChild = result.Children.First(c => c.Name == "readable");
            Assert.NotNull(readableChild);

            var unreadableChild = result.Children.First(c => c.Name == "unreadable");
            Assert.NotNull(unreadableChild);
            // Unreadable directory should still be included, but its children should be filtered
            // Since it's a directory, it's included regardless of readability
        }

        [Fact]
        public async Task GetTreeAsync_WhenNestedFileIsUnreadable_SkipsItButIncludesDirectory()
        {
            // Arrange
            string rootPath = "C:\\project";
            var mockRootInfo = new Mock<IDirectoryInfoWrapper>();
            var mockSubDir = new Mock<IDirectoryInfoWrapper>();
            var mockReadableFile = new Mock<IDirectoryInfoWrapper>();
            var mockUnreadableFile = new Mock<IDirectoryInfoWrapper>();

            mockRootInfo.Setup(x => x.FullName).Returns("C:\\project");
            mockRootInfo.Setup(x => x.Name).Returns("project");
            mockRootInfo.Setup(x => x.IsDirectory).Returns(true);

            mockSubDir.Setup(x => x.FullName).Returns("C:\\project\\subdir");
            mockSubDir.Setup(x => x.Name).Returns("subdir");
            mockSubDir.Setup(x => x.IsDirectory).Returns(true);

            mockReadableFile.Setup(x => x.FullName).Returns("C:\\project\\subdir\\readable.txt");
            mockReadableFile.Setup(x => x.Name).Returns("readable.txt");
            mockReadableFile.Setup(x => x.IsDirectory).Returns(false);

            mockUnreadableFile.Setup(x => x.FullName).Returns("C:\\project\\subdir\\unreadable.txt");
            mockUnreadableFile.Setup(x => x.Name).Returns("unreadable.txt");
            mockUnreadableFile.Setup(x => x.IsDirectory).Returns(false);

            mockSubDir.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper> { mockReadableFile.Object, mockUnreadableFile.Object });

            mockRootInfo.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper> { mockSubDir.Object });

            _mockDirectoryInfoFactory.Setup(x => x.Create(rootPath)).Returns(mockRootInfo.Object);

            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockReadableFile.Object.FullName)).Returns(true);
            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockUnreadableFile.Object.FullName)).Returns(false);

            // Act
            var result = await _fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.NotNull(result);
            var subDirNode = Assert.Single(result.Children);
            Assert.Equal("subdir", subDirNode.Name);
            Assert.Single(subDirNode.Children); // Only readable file should be included
            Assert.Equal("readable.txt", subDirNode.Children[0].Name);
        }

        #endregion

        #region CreateFileAsync Tests

        [Fact]
        public async Task CreateFileAsync_WhenDirectoryExists_CreatesFileDirectly()
        {
            // Arrange
            string path = "C:\\test\\file.txt";
            string directory = "C:\\test";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(path)).Returns(directory);
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(directory)).Returns(true);

            // Act
            await _fileSystemManager.CreateFileAsync(path);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.CreateDirectory(directory), Times.Never);
            _mockFileSystemWrapper.Verify(x => x.FileCreateAsync(path), Times.Once);
        }

        [Fact]
        public async Task CreateFileAsync_WhenDirectoryDoesNotExist_CreatesDirectoryBeforeFile()
        {
            // Arrange
            string path = "C:\\test\\sub\\file.txt";
            string directory = "C:\\test\\sub";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(path)).Returns(directory);
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(directory)).Returns(false);

            // Act
            await _fileSystemManager.CreateFileAsync(path);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.CreateDirectory(directory), Times.Once);
            _mockFileSystemWrapper.Verify(x => x.FileCreateAsync(path), Times.Once);
        }

        #endregion

        #region CreateDirectoryAsync Tests

        [Fact]
        public async Task CreateDirectoryAsync_CreatesDirectory()
        {
            // Arrange
            string path = "C:\\test\\newdir";

            // Act
            await _fileSystemManager.CreateDirectoryAsync(path);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.CreateDirectory(path), Times.Once);
        }

        #endregion

        #region DeleteFileAsync Tests

        [Fact]
        public async Task DeleteFileAsync_DeletesFile()
        {
            // Arrange
            string path = "C:\\test\\file.txt";

            // Act
            await _fileSystemManager.DeleteFileAsync(path);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.FileDelete(path), Times.Once);
        }

        #endregion

        #region DeleteDirectoryAsync Tests

        [Fact]
        public async Task DeleteDirectoryAsync_DeletesDirectory()
        {
            // Arrange
            string path = "C:\\test\\dir";

            // Act
            await _fileSystemManager.DeleteDirectoryAsync(path);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.DirectoryDelete(path), Times.Once);
        }

        #endregion

        #region MoveFileAsync Tests

        [Fact]
        public async Task MoveFileAsync_MovesFile()
        {
            // Arrange
            string sourcePath = "C:\\test\\file.txt";
            string destPath = "C:\\test\\newfile.txt";

            // Act
            await _fileSystemManager.MoveFileAsync(sourcePath, destPath);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.FileMove(sourcePath, destPath), Times.Once);
        }

        #endregion

        #region MoveDirectoryAsync Tests

        [Fact]
        public async Task MoveDirectoryAsync_MovesDirectory()
        {
            // Arrange
            string sourcePath = "C:\\test\\dir";
            string destPath = "C:\\test\\newdir";

            // Act
            await _fileSystemManager.MoveDirectoryAsync(sourcePath, destPath);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.DirectoryMove(sourcePath, destPath), Times.Once);
        }

        #endregion

        #region RenameAsync Tests

        [Fact]
        public async Task RenameAsync_WhenRenamingFile_MovesFile()
        {
            // Arrange
            string oldPath = "C:\\test\\old.txt";
            string newName = "new.txt";
            string directory = "C:\\test";
            string newPath = "C:\\test\\new.txt";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(oldPath)).Returns(directory);
            _mockFileSystemWrapper.Setup(x => x.PathCombine(directory, newName)).Returns(newPath);
            _mockFileSystemWrapper.Setup(x => x.FileExist(oldPath)).Returns(true);
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(oldPath)).Returns(false);

            // Act
            await _fileSystemManager.RenameAsync(oldPath, newName);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.FileMove(oldPath, newPath), Times.Once);
            _mockFileSystemWrapper.Verify(x => x.DirectoryMove(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RenameAsync_WhenRenamingDirectory_MovesDirectory()
        {
            // Arrange
            string oldPath = "C:\\test\\olddir";
            string newName = "newdir";
            string directory = "C:\\test";
            string newPath = "C:\\test\\newdir";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(oldPath)).Returns(directory);
            _mockFileSystemWrapper.Setup(x => x.PathCombine(directory, newName)).Returns(newPath);
            _mockFileSystemWrapper.Setup(x => x.FileExist(oldPath)).Returns(false);
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(oldPath)).Returns(true);

            // Act
            await _fileSystemManager.RenameAsync(oldPath, newName);

            // Assert
            _mockFileSystemWrapper.Verify(x => x.DirectoryMove(oldPath, newPath), Times.Once);
            _mockFileSystemWrapper.Verify(x => x.FileMove(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RenameAsync_WhenPathNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            string oldPath = "C:\\test\\nonexistent";
            string newName = "newname";
            string directory = "C:\\test";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(oldPath)).Returns(directory);
            _mockFileSystemWrapper.Setup(x => x.FileExist(oldPath)).Returns(false);
            _mockFileSystemWrapper.Setup(x => x.DirectoryExist(oldPath)).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _fileSystemManager.RenameAsync(oldPath, newName));
        }

        [Fact]
        public async Task RenameAsync_WhenPathHasNoDirectory_ThrowsArgumentException()
        {
            // Arrange
            string oldPath = "file.txt";
            string newName = "newname.txt";

            _mockFileSystemWrapper.Setup(x => x.GetDirectoryName(oldPath)).Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _fileSystemManager.RenameAsync(oldPath, newName));
        }

        #endregion

        #region FileIsReadable Integration Tests

        [Fact]
        public async Task GetTreeAsync_WhenFileIsUnreadable_ExcludesItFromTree()
        {
            // Arrange
            string rootPath = "C:\\project";
            var mockRootInfo = new Mock<IDirectoryInfoWrapper>();
            var mockFile1 = new Mock<IDirectoryInfoWrapper>();
            var mockFile2 = new Mock<IDirectoryInfoWrapper>();

            mockRootInfo.Setup(x => x.FullName).Returns("C:\\project");
            mockRootInfo.Setup(x => x.Name).Returns("project");
            mockRootInfo.Setup(x => x.IsDirectory).Returns(true);

            mockFile1.Setup(x => x.FullName).Returns("C:\\project\\file1.txt");
            mockFile1.Setup(x => x.Name).Returns("file1.txt");
            mockFile1.Setup(x => x.IsDirectory).Returns(false);

            mockFile2.Setup(x => x.FullName).Returns("C:\\project\\file2.txt");
            mockFile2.Setup(x => x.Name).Returns("file2.txt");
            mockFile2.Setup(x => x.IsDirectory).Returns(false);

            mockRootInfo.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper> { mockFile1.Object, mockFile2.Object });

            _mockDirectoryInfoFactory.Setup(x => x.Create(rootPath)).Returns(mockRootInfo.Object);

            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockFile1.Object.FullName)).Returns(true);
            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockFile2.Object.FullName)).Returns(false);

            // Act
            var result = await _fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Children);
            Assert.Equal("file1.txt", result.Children[0].Name);
            Assert.DoesNotContain(result.Children, c => c.Name == "file2.txt");
        }

        [Fact]
        public async Task GetTreeAsync_WhenFileBecomesUnreadableDuringTraversal_SkipsIt()
        {
            // Arrange
            string rootPath = "C:\\project";
            var mockRootInfo = new Mock<IDirectoryInfoWrapper>();
            var mockSubDir = new Mock<IDirectoryInfoWrapper>();
            var mockFile = new Mock<IDirectoryInfoWrapper>();

            mockRootInfo.Setup(x => x.FullName).Returns("C:\\project");
            mockRootInfo.Setup(x => x.Name).Returns("project");
            mockRootInfo.Setup(x => x.IsDirectory).Returns(true);

            mockSubDir.Setup(x => x.FullName).Returns("C:\\project\\subdir");
            mockSubDir.Setup(x => x.Name).Returns("subdir");
            mockSubDir.Setup(x => x.IsDirectory).Returns(true);

            mockFile.Setup(x => x.FullName).Returns("C:\\project\\subdir\\file.txt");
            mockFile.Setup(x => x.Name).Returns("file.txt");
            mockFile.Setup(x => x.IsDirectory).Returns(false);

            mockSubDir.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper> { mockFile.Object });

            mockRootInfo.Setup(x => x.GetFileSystemInfos())
                .Returns(new List<IDirectoryInfoWrapper> { mockSubDir.Object });

            _mockDirectoryInfoFactory.Setup(x => x.Create(rootPath)).Returns(mockRootInfo.Object);

            _mockFileSystemWrapper.Setup(x => x.FileIsReadable(mockFile.Object.FullName)).Returns(false);

            // Act
            var result = await _fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.NotNull(result);
            var subDirNode = Assert.Single(result.Children);
            Assert.Empty(subDirNode.Children);
        }

        #endregion
    }
}
