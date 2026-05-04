using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Services
{
    public class CatalogChanged(ICatalogChangedHandler handler) : INotificationHandler<CatalogChangedNotification>
    {
        public async Task Handle(CatalogChangedNotification notification, CancellationToken cancellationToken)
        {
            handler.CatalogChanged?.Invoke();
        }
    }

    public class CatalogChangedHandler : ICatalogChangedHandler
    {
        public Action? CatalogChanged { get; set; }
    }
}
