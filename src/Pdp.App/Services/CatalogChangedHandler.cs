using MediatR;
using Pdp.App.Interfaces;
using Pdp.App.Models;

namespace Pdp.App.Services
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
