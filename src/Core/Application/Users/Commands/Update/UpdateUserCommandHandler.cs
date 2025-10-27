using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Users.Commands.Update
{
    internal sealed class UpdateUserCommandHandler(
        IManagersService managersService,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService) : IRequestHandler<UpdateUserCommand, string>
    {
        public async Task<string> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            AppUser? user = await managersService.FindByIdAsync(request.Id) ??
                throw new NotFoundException(nameof(AppUser), request.Id);

            if ((!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email) ||
                (!string.IsNullOrWhiteSpace(request.UserName) && request.UserName != user.UserName))
            {
                bool userExists = await managersService.UserExistsExcludingAsync(
                    request.UserName ?? user.UserName ?? string.Empty,
                    request.Email ?? user.Email ?? string.Empty,
                    request.Id,
                    cancellationToken);

                if (userExists)
                    throw new BadRequestException(Resources.Translation.UserExists);
            }

            UpdateUserProperties(request, user);

            Result updateResult = await managersService.UpdateUserAsync(user);

            if (!updateResult.Succeeded)
                throw new BadRequestException(string.Concat(updateResult.Errors));

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                Result passwordResult = await managersService.UpdatePasswordAsync(user, request.Password);

                if (!passwordResult.Succeeded)
                    throw new BadRequestException(string.Concat(passwordResult.Errors));
            }

            return user.Id;
        }

        private void UpdateUserProperties(UpdateUserCommand request, AppUser user)
        {
            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(request.FirstName) && request.FirstName != user.FirstName)
            {
                user.FirstName = request.FirstName;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.LastName) && request.LastName != user.LastName)
            {
                user.LastName = request.LastName;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.UserName) && request.UserName != user.UserName)
            {
                user.UserName = request.UserName;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                user.Email = request.Email;
                hasChanges = true;
            }

            if (request.LanguageId.HasValue && request.LanguageId != user.LanguageId)
            {
                user.LanguageId = request.LanguageId;
                hasChanges = true;
            }

            if (hasChanges)
            {
                user.LastModified = dateTimeService.Now;
                user.LastModifiedBy = currentUserService.UserId;
            }
        }
    }
}