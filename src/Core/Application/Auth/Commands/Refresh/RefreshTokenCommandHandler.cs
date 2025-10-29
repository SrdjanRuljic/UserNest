using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.Refresh
{
    internal sealed class RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtFactory jwtFactory,
        IManagersService managersService) : IRequestHandler<RefreshTokenCommand, RefreshDto>
    {
        public async Task<RefreshDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            bool isValid = await jwtFactory.ValidateAsync(request.RefreshToken);

            if (!isValid)
                throw new BadRequestException(Resources.Translation.InvalidRefreshToken);

            RefreshToken? refreshToken = await context.RefreshTokens
                .Where(rt => rt.Token == request.RefreshToken)
                .FirstOrDefaultAsync(cancellationToken) ??
                throw new NotFoundException(ErrorMessages.CreateEntityWasNotFoundMessage(nameof(RefreshToken), request.RefreshToken));

            context.RefreshTokens.Remove(refreshToken);

            AppUser? user = await managersService.FindByIdAsync(refreshToken.UserId) ??
                throw new NotFoundException(string.Format(ErrorMessages.CreateEntityWasNotFoundMessage(nameof(AppUser), refreshToken.UserId)));

            string[] roles = await managersService.GetRolesAsync(user);

            TokenResponse tokens = TokenHelper.GenerateJwt(user.Id, roles, jwtFactory);

            await context.RefreshTokens
                .AddAsync(new RefreshToken(
                    user.Id,
                    tokens.RefreshToken), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new RefreshDto(tokens.AuthToken, tokens.RefreshToken);
        }
    }
}