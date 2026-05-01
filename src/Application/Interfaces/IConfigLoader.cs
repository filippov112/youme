using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IConfigLoader
    {
        public Task<Config> LoadGlobal();
        public Task<Config> LoadLocal(string projectDirectory);
        public Task SaveLocal(Config local, string projectDirectory);
        public Task SaveGlobal(Config global);
    }
}
