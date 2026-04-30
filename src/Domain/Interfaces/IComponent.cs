using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IComponent
    {
        public string Key { get; }
        public string Value { get; }
    }
}
