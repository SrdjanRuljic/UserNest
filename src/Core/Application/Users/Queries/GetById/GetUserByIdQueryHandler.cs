using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetById
{
    internal sealed class GetUserByIdQueryHandler(
        IManagersService managersService,
        IMapper mapper) : IRequestHandler<GetUserByIdQuery, GetUserByIdDto>
    {
        public async Task<GetUserByIdDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            GetUserByIdDto model = await managersService.GetUsers()
                .Where(x => x.Id == request.Id)
                .ProjectTo<GetUserByIdDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken) ??
                throw new NotFoundException(nameof(GetUserByIdDto), request.Id);

            return model;
        }
    }
}