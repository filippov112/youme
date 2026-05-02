using Application.Interfaces;
using Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Queries
{

    public record GetSettingsQuery() : IRequest<AllConfigDto>;

    public class GetSettingsQueryHandler(IConfigService cs) : IRequestHandler<GetSettingsQuery, AllConfigDto>
    {
        public async Task<AllConfigDto> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            return await cs.GetAllConfigAsync();
        }
    }
}
