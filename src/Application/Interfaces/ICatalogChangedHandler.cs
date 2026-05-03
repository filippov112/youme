using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICatalogChangedHandler
    {
        public event Action CatalogChanged;
    }
}
