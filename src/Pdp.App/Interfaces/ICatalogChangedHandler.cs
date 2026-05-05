namespace Pdp.App.Interfaces
{
    public interface ICatalogChangedHandler
    {
        public Action? CatalogChanged { get; set; }
    }
}
