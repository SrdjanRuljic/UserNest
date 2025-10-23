using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Auth.Commands.Logout
{
    internal sealed class LogoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IManagersService managersService) : IRequestHandler<LogoutCommand>
    {
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            await managersService.SignOutAsync();

            RefreshToken refreshToken = await context.RefreshTokens.FindAsync([
                currentUserService.UserId,
                request.RefreshToken
            ], cancellationToken) ??
                throw new NotFoundException(ErrorMessages.CreateEntityWasNotFoundMessage(nameof(AppUser), currentUserService.UserId));

            context.RefreshTokens.Remove(refreshToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}