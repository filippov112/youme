using Application.Interfaces;
using Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Queries
{
    public record GetDefaultSettingsQuery() : IRequest<AllConfigDto>;

    public class GetDefaultSettingsQueryHandler(IConfigService cs) : IRequestHandler<GetDefaultSettingsQuery, AllConfigDto>
    {
        public async Task<AllConfigDto> Handle(GetDefaultSettingsQuery request, CancellationToken cancellationToken)
        {
            return cs.GetAllConfigDefault();
        }
    }
}
