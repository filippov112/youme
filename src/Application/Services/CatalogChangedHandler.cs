using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Services
{
    public class CatalogChangedHandler : ICatalogChangedHandler, INotificationHandler<CatalogChangedNotification>
    {
        public event Action? CatalogChanged;
        public async Task Handle(CatalogChangedNotification notification, CancellationToken cancellationToken)
        {
            CatalogChanged?.Invoke();
        }
    }
}
