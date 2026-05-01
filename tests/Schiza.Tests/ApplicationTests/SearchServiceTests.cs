using Application.Interfaces;
using Application.Services;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Schiza.Tests.ApplicationTests
{
    public class SearchServiceTests
    {
        private readonly Mock<IFileSystemManager> _fsm;
        private readonly Mock<IConfigService> _cs;
        private readonly ISearchService _searchService;

        private readonly ProjectUnit _file1;
        private readonly ProjectUnit _file2;
        private readonly ProjectUnit _file3;
        private readonly ProjectUnit _file4;
        private readonly ProjectUnit _file5;
        private readonly ProjectUnit _file6;
        private readonly ProjectUnit _file7;
        private readonly ProjectUnit _file8;

        private readonly ProjectUnit _cat1;
        private readonly ProjectUnit _cat2;
        private readonly ProjectUnit _cat3;
        private readonly ProjectUnit _cat4;

        public SearchServiceTests()
        {
            string path = "root_catalog";
            _file1 = new ProjectUnit() { Name = "dog.txt" };
            _file2 = new ProjectUnit() { Name = "cat.txt" };
            _file3 = new ProjectUnit() { Name = "elephant.txt" };
            _file4 = new ProjectUnit() { Name = "rat.txt" };
            _file5 = new ProjectUnit() { Name = "mouse.txt" };
            _file6 = new ProjectUnit() { Name = "bird.txt" };
            _file7 = new ProjectUnit() { Name = "fox.txt" };
            _file8 = new ProjectUnit() { Name = "wolf.txt" };

            _cat4 = new ProjectUnit() { IsDirectory = true, Name = "cat", Children = [_file3, _file4] };
            _cat1 = new ProjectUnit() { IsDirectory = true, Name = "group1", Children = [_cat4, _file2, _file1] };
            _cat2 = new ProjectUnit() { IsDirectory = true, Name = "group2", Children = [_file5, _file6, _file7] };
            _cat3 = new ProjectUnit() { IsDirectory = true, Name = "group0", Children = [_file8, _cat1, _cat2] };

            _fsm = new Mock<IFileSystemManager>();
            _fsm.Setup(x => x.GetTreeAsync(path)).ReturnsAsync(_cat3);
            _cs = new Mock<IConfigService>();
            _cs.Setup(x => x.RootDirectory).Returns(path);

            _searchService = new SearchService(_fsm.Object, _cs.Object);
        }

        [Fact]
        public async Task FindMatches_ReturnCorrectlyResult1()
        {
            string pattern1 = "cat";

            var result1 = await _searchService.FindMatches(pattern1);

            Assert.NotNull(result1);
            Assert.Equal(CollectProjectUnits([], result1), [_file2, _cat1, _cat3]);
        }

        [Fact]
        public async Task FindMatches_ReturnCorrectlyResult2()
        {
            string pattern2 = "bird";

            var result2 = await _searchService.FindMatches(pattern2);

            Assert.NotNull(result2);
            Assert.Equal(CollectProjectUnits([], result2), [_file6, _cat2, _cat3]);
        }

        [Fact]
        public async Task FindMatches_ReturnCorrectlyResult3()
        {
            string pattern3 = "windows";

            var result3 = await _searchService.FindMatches(pattern3);

            Assert.Null(result3);
        }

        private HashSet<ProjectUnit> CollectProjectUnits(HashSet<ProjectUnit> container, ProjectUnit unit)
        {
            container.Add(unit);
            foreach (var child in unit.Children)
                CollectProjectUnits(container, child);
            return container;
        }
    }
}
