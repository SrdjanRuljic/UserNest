using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Auth.Commands.Login
{
    internal sealed class LoginCommandHandler(
        IApplicationDbContext context,
        IJwtFactory jwtFactory,
        IManagersService managersService,
        IMapper mapper) : IRequestHandler<LoginCommand, LoginDto>
    {
        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            AppUser? user = await managersService.AuthenticateAsync(
                request.Username,
                request.Password,
                cancellationToken) ??
                throw new BadRequestException(Resources.Translation.IncorrectUsernameOrPassword);

            string[] roles = await managersService.GetRolesAsync(user);

            TokenResponse tokens = TokenHelper.GenerateJwt(
                user.Id,
                roles,
                jwtFactory);

            await context.RefreshTokens
                .AddAsync(new RefreshToken(
                    user.Id,
                    tokens.RefreshToken), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new LoginDto(
                tokens.AuthToken,
                tokens.RefreshToken);
        }
    }
}