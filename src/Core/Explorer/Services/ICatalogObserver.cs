namespace Core.Explorer.Services
{
    public interface ICatalogObserver
    {
        public void StartObserving();
        public void StopObserving();
    }
}
