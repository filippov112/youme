using Core.Explorer.Models;
using MediatR;

namespace Core.Explorer.Services
{
    public interface ICatalogChangeEvent
    {
        public Action? CatalogChanged { get; set; }
    }


    /// <summary>
    /// Объект, удержваемый подписчиками
    /// </summary>
    public class CatalogChangedHandler : ICatalogChangeEvent
    {
        public Action? CatalogChanged { get; set; }
    }


    /// <summary>
    /// Ссылка на CatalogChangedHandler для MediatR, который триггерит событие, за которым следят подписчики
    /// </summary>
    /// <param name="handler"></param>
    public class CatalogChanged(ICatalogChangeEvent handler) : INotificationHandler<CatalogChangedNotification>
    {
        public async Task Handle(CatalogChangedNotification notification, CancellationToken cancellationToken)
        {
            handler.CatalogChanged?.Invoke();
        }
    }
}
