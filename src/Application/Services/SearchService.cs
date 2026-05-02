using Application.Interfaces;
using Application.Models;

namespace Application.Services
{
    public class SearchService(IFileSystemManager fsm, IConfigService config) : ISearchService
    {

        public async Task<ProjectUnit?> FindMatches(string pattern)
        {
            ProjectUnit? tree = await fsm.GetTreeAsync(config.RootDirectory);
            if (tree == null)
                return null;
            return Find(tree, pattern);
        }

        private ProjectUnit? Find(ProjectUnit unit, string pattern)
        {
            // Листья (файлы)
            if (!unit.IsDirectory) 
                return unit.Name.Contains(pattern) ? unit : null;

            List<ProjectUnit> trueChildren = [];
            // Узлы (каталоги)
            foreach(var child in unit.Children)
            {
                var childUnit = Find(child, pattern);
                if (childUnit != null)
                    trueChildren.Add(childUnit);
            }
            unit.Children = trueChildren;
            if (trueChildren.Count > 0)
                return unit;
            return null;
        }
    }
}
