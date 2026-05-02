using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Schiza.Tests.IntegrationTests
{
    public class CommandQueryIntegrationTests
    {
        private readonly Mock<IFileSystemWrapper> _fileSystemWrapperMock;
        private readonly Mock<IDirectoryInfoWrapper> _directoryInfoWrapperMock;
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly Mock<ICatalogObserver> _catalogObserverMock;
        private readonly Mock<ISearchService> _searchServiceMock;
        private readonly Mock<ITokenCounter> _tokenCounterMock;
        private readonly Mock<IFileSystemManager> _fileSystemManagerMock;
        private readonly Mock<IMediator> _mediatorMock;

        public CommandQueryIntegrationTests()
        {
            _fileSystemWrapperMock = new Mock<IFileSystemWrapper>();
            _directoryInfoWrapperMock = new Mock<IDirectoryInfoWrapper>();
            _configServiceMock = new Mock<IConfigService>();
            _catalogObserverMock = new Mock<ICatalogObserver>();
            _searchServiceMock = new Mock<ISearchService>();
            _tokenCounterMock = new Mock<ITokenCounter>();
            _mediatorMock = new Mock<IMediator>();

            // Setup FileSystemManager with mocks
            _fileSystemManagerMock = new Mock<IFileSystemManager>();
        }

        #region Command Tests

        [Fact]
        public async Task ChangePathCommandHandler_Should_Call_ChangePathAsync()
        {
            // Arrange
            var command = new ChangePathCommand(@"C:\old\file.txt", "newfile.txt");
            _fileSystemManagerMock.Setup(x => x.ChangePathAsync(command.OldPath, command.NewPath))
                .Returns(Task.CompletedTask);

            var handler = new ChangePathCommandHandler(_fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
            _fileSystemManagerMock.Verify(x => x.ChangePathAsync(command.OldPath, command.NewPath), Times.Once);
        }

        [Fact]
        public async Task CreateDirCommandHandler_Should_Call_CreateDirectoryAsync()
        {
            // Arrange
            var command = new CreateDirCommand(@"C:\test\newdir");
            _fileSystemManagerMock.Setup(x => x.CreateDirectoryAsync(command.Path))
                .Returns(Task.CompletedTask);

            var handler = new CreateDirCommandHandler(_fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
            _fileSystemManagerMock.Verify(x => x.CreateDirectoryAsync(command.Path), Times.Once);
        }

        [Fact]
        public async Task DeleteCommandHandler_Should_Call_DeleteAsync_ForFile()
        {
            // Arrange
            var command = new DeleteCommand(@"C:\test\file.txt");
            _fileSystemManagerMock.Setup(x => x.DeleteAsync(command.Path))
                .Returns(Task.CompletedTask);

            var handler = new DeleteCommandHandler(_fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
            _fileSystemManagerMock.Verify(x => x.DeleteAsync(command.Path), Times.Once);
        }

        [Fact]
        public async Task DeleteCommandHandler_Should_Call_DeleteAsync_ForDirectory()
        {
            // Arrange
            var command = new DeleteCommand(@"C:\test\directory");
            _fileSystemManagerMock.Setup(x => x.DeleteAsync(command.Path))
                .Returns(Task.CompletedTask);

            var handler = new DeleteCommandHandler(_fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
            _fileSystemManagerMock.Verify(x => x.DeleteAsync(command.Path), Times.Once);
        }

        [Fact]
        public async Task OpenProjectCommandHandler_Should_SetRootDirectory_And_StartObserving()
        {
            // Arrange
            var command = new OpenProjectCommand(@"C:\myproject");
            _configServiceMock.Setup(x => x.SetRootDirectoryAsync(command.RootDirectory))
                .Returns(Task.CompletedTask);

            var handler = new OpenProjectCommandHandler(_configServiceMock.Object, _catalogObserverMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
            _configServiceMock.Verify(x => x.SetRootDirectoryAsync(command.RootDirectory), Times.Once);
            _catalogObserverMock.Verify(x => x.StartObserving(), Times.Once);
        }

        [Fact]
        public async Task SaveSettingsCommandHandler_Should_Call_SaveAllConfigAsync()
        {
            // Arrange
            var config = new AllConfigDto
            {
                Global = new CombinationConfig(),
                Local = new CombinationConfig()
            };
            var command = new SaveSettingsCommand(config);
            _configServiceMock.Setup(x => x.SaveAllConfigAsync(config))
                .Returns(Task.CompletedTask);

            var handler = new SaveSettingsCommandHandler(_configServiceMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
            _configServiceMock.Verify(x => x.SaveAllConfigAsync(config), Times.Once);
        }

        [Fact]
        public async Task WriteFileCommandHandler_Should_Call_WriteFileAsync()
        {
            // Arrange
            var command = new WriteFileCommand(@"C:\test\file.txt", "Hello World");
            _fileSystemManagerMock.Setup(x => x.WriteFileAsync(command.Path, command.Content))
                .Returns(Task.CompletedTask);

            var handler = new WriteFileCommandHandler(_fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
            _fileSystemManagerMock.Verify(x => x.WriteFileAsync(command.Path, command.Content), Times.Once);
        }

        #endregion

        #region Query Tests

        [Fact]
        public async Task GetDefaultSettingsQueryHandler_Should_Return_DefaultConfig()
        {
            // Arrange
            var expectedConfig = new AllConfigDto
            {
                Global = new CombinationConfig(),
                Local = new CombinationConfig()
            };
            _configServiceMock.Setup(x => x.GetAllConfigDefault())
                .Returns(expectedConfig);

            var query = new GetDefaultSettingsQuery();
            var handler = new GetDefaultSettingsQueryHandler(_configServiceMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedConfig, result);
            _configServiceMock.Verify(x => x.GetAllConfigDefault(), Times.Once);
        }

        [Fact]
        public async Task GetFilteredTreeQueryHandler_Should_Return_FilteredTree()
        {
            // Arrange
            var pattern = "test";
            var expectedTree = new ProjectUnit
            {
                Name = "root",
                Path = @"C:\root",
                IsDirectory = true,
                Children = new List<ProjectUnit>
                {
                    new ProjectUnit { Name = "test.txt", Path = @"C:\root\test.txt", IsDirectory = false }
                }
            };
            _searchServiceMock.Setup(x => x.FindMatchesAsync(pattern))
                .ReturnsAsync(expectedTree);

            var query = new GetFilteredTreeQuery(pattern);
            var handler = new GetFilteredTreeQueryHandler(_searchServiceMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedTree, result);
            _searchServiceMock.Verify(x => x.FindMatchesAsync(pattern), Times.Once);
        }

        [Fact]
        public async Task GetPromptQueryHandler_Should_Build_Prompt_Correctly()
        {
            // Arrange
            var filePaths = new List<string> { @"C:\test\file1.txt", @"C:\test\file2.txt" };
            var queryText = "What is this code?";
            var query = new GetPromptQuery(filePaths, queryText);

            var combinationConfig = new CombinationConfig
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "##intro##\n```\n##context##\n```\n##query##\n\n##rules##",
                FileStructure = "File: ##path##\n```\n##content##\n```",
                IntroductionText = "You are a helpful assistant",
                RulesText = "Be concise and accurate"
            };

            _configServiceMock.Setup(x => x.GetCombinationConfigAsync())
                .ReturnsAsync(combinationConfig);

            _fileSystemManagerMock.Setup(x => x.ReadFileAsync(filePaths[0]))
                .ReturnsAsync("Content of file1");
            _fileSystemManagerMock.Setup(x => x.ReadFileAsync(filePaths[1]))
                .ReturnsAsync("Content of file2");

            var handler = new GetPromptQueryHandler(_configServiceMock.Object, _fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Contains("You are a helpful assistant", result);
            Assert.Contains("Content of file1", result);
            Assert.Contains("Content of file2", result);
            Assert.Contains("What is this code?", result);
            Assert.Contains("Be concise and accurate", result);

            _configServiceMock.Verify(x => x.GetCombinationConfigAsync(), Times.Once);
            _fileSystemManagerMock.Verify(x => x.ReadFileAsync(filePaths[0]), Times.Once);
            _fileSystemManagerMock.Verify(x => x.ReadFileAsync(filePaths[1]), Times.Once);
        }

        [Fact]
        public async Task GetSettingsQueryHandler_Should_Return_AllConfig()
        {
            // Arrange
            var expectedConfig = new AllConfigDto
            {
                Global = new CombinationConfig(),
                Local = new CombinationConfig()
            };
            _configServiceMock.Setup(x => x.GetAllConfigAsync())
                .ReturnsAsync(expectedConfig);

            var query = new GetSettingsQuery();
            var handler = new GetSettingsQueryHandler(_configServiceMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedConfig, result);
            _configServiceMock.Verify(x => x.GetAllConfigAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTextQueryHandler_Should_Return_FileContent()
        {
            // Arrange
            var path = @"C:\test\file.txt";
            var expectedContent = "File content here";
            var query = new GetTextQuery(path);

            _fileSystemManagerMock.Setup(x => x.ReadFileAsync(path))
                .ReturnsAsync(expectedContent);

            var handler = new GetTextQueryHandler(_fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedContent, result);
            _fileSystemManagerMock.Verify(x => x.ReadFileAsync(path), Times.Once);
        }

        [Fact]
        public async Task GetTokenCountQueryHandler_Should_Return_TokenCount()
        {
            // Arrange
            var prompt = "This is a sample prompt for token counting";
            var expectedTokenCount = 8;
            var query = new GetTokenCountQuery(prompt);

            _tokenCounterMock.Setup(x => x.CalcTokenCount(prompt))
                .Returns(expectedTokenCount);

            var handler = new GetTokenCountQueryHandler(_tokenCounterMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedTokenCount, result);
            _tokenCounterMock.Verify(x => x.CalcTokenCount(prompt), Times.Once);
        }

        [Fact]
        public async Task GetTreeQueryHandler_Should_Return_DirectoryTree()
        {
            // Arrange
            var rootDirectory = @"C:\myproject";
            var expectedTree = new ProjectUnit
            {
                Name = "myproject",
                Path = rootDirectory,
                IsDirectory = true,
                Children = new List<ProjectUnit>
                {
                    new ProjectUnit { Name = "src", Path = @"C:\myproject\src", IsDirectory = true },
                    new ProjectUnit { Name = "file.txt", Path = @"C:\myproject\file.txt", IsDirectory = false }
                }
            };

            _configServiceMock.Setup(x => x.RootDirectory)
                .Returns(rootDirectory);
            _fileSystemManagerMock.Setup(x => x.GetTreeAsync(rootDirectory))
                .ReturnsAsync(expectedTree);

            var query = new GetTreeQuery();
            var handler = new GetTreeQueryHandler(_configServiceMock.Object, _fileSystemManagerMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedTree, result);
            _configServiceMock.Verify(x => x.RootDirectory, Times.AtLeastOnce);
            _fileSystemManagerMock.Verify(x => x.GetTreeAsync(rootDirectory), Times.Once);
        }

        #endregion

        #region FileSystemManager Integration Tests (with mocked wrappers)

        [Fact]
        public async Task FileSystemManager_ReadFileAsync_Should_Return_FileContent()
        {
            // Arrange
            var path = @"C:\test\file.txt";
            var expectedContent = "Test content";
            _fileSystemWrapperMock.Setup(x => x.FileReadAsync(path))
                .ReturnsAsync(expectedContent);

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act
            var result = await fileSystemManager.ReadFileAsync(path);

            // Assert
            Assert.Equal(expectedContent, result);
            _fileSystemWrapperMock.Verify(x => x.FileReadAsync(path), Times.Once);
        }

        [Fact]
        public async Task FileSystemManager_WriteFileAsync_Should_CreateDirectory_IfNotExists()
        {
            // Arrange
            var path = @"C:\test\subdir\file.txt";
            var content = "Test content";
            var directory = @"C:\test\subdir";

            _fileSystemWrapperMock.Setup(x => x.GetDirectoryName(path))
                .Returns(directory);
            _fileSystemWrapperMock.Setup(x => x.DirectoryExist(directory))
                .Returns(false);
            _fileSystemWrapperMock.Setup(x => x.CreateDirectory(directory));
            _fileSystemWrapperMock.Setup(x => x.FileWriteAsync(path, content))
                .Returns(Task.CompletedTask);

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act
            await fileSystemManager.WriteFileAsync(path, content);

            // Assert
            _fileSystemWrapperMock.Verify(x => x.GetDirectoryName(path), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.DirectoryExist(directory), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.CreateDirectory(directory), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.FileWriteAsync(path, content), Times.Once);
        }

        [Fact]
        public async Task FileSystemManager_DeleteAsync_Should_Delete_File()
        {
            // Arrange
            var path = @"C:\test\file.txt";
            _fileSystemWrapperMock.Setup(x => x.FileExist(path))
                .Returns(true);
            _fileSystemWrapperMock.Setup(x => x.FileDelete(path));

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act
            await fileSystemManager.DeleteAsync(path);

            // Assert
            _fileSystemWrapperMock.Verify(x => x.FileExist(path), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.FileDelete(path), Times.Once);
        }

        [Fact]
        public async Task FileSystemManager_DeleteAsync_Should_Delete_Directory()
        {
            // Arrange
            var path = @"C:\test\directory";
            _fileSystemWrapperMock.Setup(x => x.FileExist(path))
                .Returns(false);
            _fileSystemWrapperMock.Setup(x => x.DirectoryExist(path))
                .Returns(true);
            _fileSystemWrapperMock.Setup(x => x.DirectoryDelete(path));

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act
            await fileSystemManager.DeleteAsync(path);

            // Assert
            _fileSystemWrapperMock.Verify(x => x.FileExist(path), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.DirectoryExist(path), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.DirectoryDelete(path), Times.Once);
        }

        [Fact]
        public async Task FileSystemManager_DeleteAsync_Should_Throw_If_Path_Not_Found()
        {
            // Arrange
            var path = @"C:\test\nonexistent";
            _fileSystemWrapperMock.Setup(x => x.FileExist(path))
                .Returns(false);
            _fileSystemWrapperMock.Setup(x => x.DirectoryExist(path))
                .Returns(false);

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(
                () => fileSystemManager.DeleteAsync(path));
        }

        [Fact]
        public async Task FileSystemManager_ChangePathAsync_Should_Move_File()
        {
            // Arrange
            var oldPath = @"C:\test\oldname.txt";
            var newName = "newname.txt";
            var directory = @"C:\test";
            var newPath = @"C:\test\newname.txt";

            _fileSystemWrapperMock.Setup(x => x.GetDirectoryName(oldPath))
                .Returns(directory);
            _fileSystemWrapperMock.Setup(x => x.PathCombine(directory, newName))
                .Returns(newPath);
            _fileSystemWrapperMock.Setup(x => x.FileExist(oldPath))
                .Returns(true);
            _fileSystemWrapperMock.Setup(x => x.FileMove(oldPath, newPath));

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act
            await fileSystemManager.ChangePathAsync(oldPath, newName);

            // Assert
            _fileSystemWrapperMock.Verify(x => x.GetDirectoryName(oldPath), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.PathCombine(directory, newName), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.FileExist(oldPath), Times.Once);
            _fileSystemWrapperMock.Verify(x => x.FileMove(oldPath, newPath), Times.Once);
        }

        [Fact]
        public async Task FileSystemManager_ChangePathAsync_Should_Move_Directory()
        {
            // Arrange
            var oldPath = @"C:\test\olddir";
            var newName = "newdir";
            var directory = @"C:\test";
            var newPath = @"C:\test\newdir";

            _fileSystemWrapperMock.Setup(x => x.GetDirectoryName(oldPath))
                .Returns(directory);
            _fileSystemWrapperMock.Setup(x => x.PathCombine(directory, newName))
                .Returns(newPath);
            _fileSystemWrapperMock.Setup(x => x.FileExist(oldPath))
                .Returns(false);
            _fileSystemWrapperMock.Setup(x => x.DirectoryExist(oldPath))
                .Returns(true);
            _fileSystemWrapperMock.Setup(x => x.DirectoryMove(oldPath, newPath));

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act
            await fileSystemManager.ChangePathAsync(oldPath, newName);

            // Assert
            _fileSystemWrapperMock.Verify(x => x.DirectoryMove(oldPath, newPath), Times.Once);
        }

        [Fact]
        public async Task FileSystemManager_GetTreeAsync_Should_Return_Null_If_Not_Readable()
        {
            // Arrange
            var rootPath = @"C:\test";
            var rootInfoMock = new Mock<IDirectoryInfoWrapper>();

            _directoryInfoWrapperMock.Setup(x => x.Create(rootPath))
                .Returns(rootInfoMock.Object);
            rootInfoMock.Setup(x => x.IsDirectory).Returns(false);
            _fileSystemWrapperMock.Setup(x => x.FileIsReadable(rootPath))
                .Returns(false);

            var fileSystemManager = new Infrastructure.Services.FileSystemManager(
                _fileSystemWrapperMock.Object,
                _directoryInfoWrapperMock.Object);

            // Act
            var result = await fileSystemManager.GetTreeAsync(rootPath);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region ConfigService Tests (with mocked IConfigLoader)

        [Fact]
        public async Task ConfigService_GetCombinationConfigAsync_Should_Return_CombinationConfig()
        {
            // Arrange
            var configLoaderMock = new Mock<IConfigLoader>();
            var globalConfig = new Config
            {
                IntroductionKey = "##intro##",
                ContextKey = "##context##",
                RulesKey = "##rules##",
                QueryKey = "##query##",
                FilePathKey = "##path##",
                FileContentKey = "##content##",
                PromptStructure = "Test structure",
                FileStructure = "File structure",
                IntroductionDef = "Intro text",
                RulesDef = "Rules text"
            };

            configLoaderMock.Setup(x => x.LoadGlobal())
                .ReturnsAsync(globalConfig);

            var configService = new Application.Services.ConfigService(configLoaderMock.Object);

            // Act
            var result = await configService.GetCombinationConfigAsync();

            // Assert
            Assert.Equal(globalConfig.IntroductionKey, result.IntroductionKey);
            Assert.Equal(globalConfig.ContextKey, result.ContextKey);
            Assert.Equal(globalConfig.RulesKey, result.RulesKey);
            Assert.Equal(globalConfig.QueryKey, result.QueryKey);
            Assert.Equal(globalConfig.FilePathKey, result.FilePathKey);
            Assert.Equal(globalConfig.FileContentKey, result.FileContentKey);
            Assert.Equal(globalConfig.PromptStructure, result.PromptStructure);
            Assert.Equal(globalConfig.FileStructure, result.FileStructure);
            Assert.Equal(globalConfig.IntroductionDef, result.IntroductionText);
            Assert.Equal(globalConfig.RulesDef, result.RulesText);
        }

        [Fact]
        public async Task ConfigService_SetRootDirectoryAsync_Should_Load_Local_Config()
        {
            // Arrange
            var configLoaderMock = new Mock<IConfigLoader>();
            var rootDirectory = @"C:\myproject";
            var localConfig = new Config
            {
                IntroductionKey = "local_intro",
                ContextKey = "local_context"
            };

            configLoaderMock.Setup(x => x.LoadGlobal())
                .ReturnsAsync(new Config());
            configLoaderMock.Setup(x => x.LoadLocal(rootDirectory))
                .ReturnsAsync(localConfig);

            var configService = new Application.Services.ConfigService(configLoaderMock.Object);

            // Act
            await configService.SetRootDirectoryAsync(rootDirectory);

            // Assert
            Assert.Equal(rootDirectory, configService.RootDirectory);

            var result = await configService.GetCombinationConfigAsync();
            Assert.Equal("local_intro", result.IntroductionKey);
            Assert.Equal("local_context", result.ContextKey);
        }

        #endregion

        #region SearchService Tests

        [Fact]
        public async Task SearchService_FindMatchesAsync_Should_Filter_Tree_By_Pattern()
        {
            // Arrange
            var rootDirectory = @"C:\test";
            var pattern = "test";
            var fullTree = new ProjectUnit
            {
                Name = "root",
                Path = rootDirectory,
                IsDirectory = true,
                Children = new List<ProjectUnit>
                {
                    new ProjectUnit { Name = "test.txt", Path = @"C:\test\test.txt", IsDirectory = false },
                    new ProjectUnit { Name = "other.txt", Path = @"C:\test\other.txt", IsDirectory = false },
                    new ProjectUnit
                    {
                        Name = "subdir",
                        Path = @"C:\test\subdir",
                        IsDirectory = true,
                        Children = new List<ProjectUnit>
                        {
                            new ProjectUnit { Name = "test2.txt", Path = @"C:\test\subdir\test2.txt", IsDirectory = false },
                            new ProjectUnit { Name = "data.txt", Path = @"C:\test\subdir\data.txt", IsDirectory = false }
                        }
                    }
                }
            };

            _configServiceMock.Setup(x => x.RootDirectory)
                .Returns(rootDirectory);
            _fileSystemManagerMock.Setup(x => x.GetTreeAsync(rootDirectory))
                .ReturnsAsync(fullTree);

            var searchService = new Application.Services.SearchService(
                _fileSystemManagerMock.Object,
                _configServiceMock.Object);

            // Act
            var result = await searchService.FindMatchesAsync(pattern);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("root", result.Name);
            Assert.Equal(2, result.Children.Count); // test.txt and subdir (which contains test2.txt)

            var testFile = result.Children.FirstOrDefault(c => c.Name == "test.txt");
            Assert.NotNull(testFile);

            var subdir = result.Children.FirstOrDefault(c => c.Name == "subdir");
            Assert.NotNull(subdir);
            Assert.Single(subdir.Children); // only test2.txt
            Assert.Equal("test2.txt", subdir.Children[0].Name);
        }

        [Fact]
        public async Task SearchService_FindMatchesAsync_Should_Return_Null_If_Tree_Is_Null()
        {
            // Arrange
            var rootDirectory = @"C:\test";
            var pattern = "test";

            _configServiceMock.Setup(x => x.RootDirectory)
                .Returns(rootDirectory);
            _fileSystemManagerMock.Setup(x => x.GetTreeAsync(rootDirectory))
                .ReturnsAsync((ProjectUnit?)null);

            var searchService = new Application.Services.SearchService(
                _fileSystemManagerMock.Object,
                _configServiceMock.Object);

            // Act
            var result = await searchService.FindMatchesAsync(pattern);

            // Assert
            Assert.Null(result);
        }

        #endregion
    }
}