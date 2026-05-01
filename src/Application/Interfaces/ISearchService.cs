using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ISearchService
    {
        public Task<ProjectUnit?> FindMatches(string pattern);
    }
}
