namespace Application.Interfaces
{
    public interface ICatalogChangedHandler
    {
        public Action? CatalogChanged { get; set; }
    }
}
