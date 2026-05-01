using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICatalogObserver
    {
        public void StartObserving();
        public void StopObserving();
    }
}
