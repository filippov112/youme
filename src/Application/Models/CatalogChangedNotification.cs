using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public record CatalogChangedNotification : INotification;
}
