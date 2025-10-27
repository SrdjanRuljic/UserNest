using Application.Common.Interfaces;
using Application.Exceptions;
using MediatR;
using Domain.Entities.Identity;

namespace Application.Users.Queries.ValidatePassword
{
    internal sealed class ValidatePasswordQueryHandler(
        IManagersService managersService,
        ICurrentUserService currentUserService) : IRequestHandler<ValidatePasswordQuery, bool>
    {
        public async Task<bool> Handle(ValidatePasswordQuery request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            AppUser? user = await managersService.FindByIdAsync(currentUserService.UserId) ??
                throw new NotFoundException(nameof(AppUser), currentUserService.UserId);

            bool isValidPassword = await managersService.ValidatePasswordAsync(user, request.Password);

            return isValidPassword;
        }
    }
}
