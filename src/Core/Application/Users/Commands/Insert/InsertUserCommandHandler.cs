using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Users.Commands.Insert
{
    internal sealed class InsertUserCommandHandler(
        IManagersService managersService,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService) : IRequestHandler<InsertUserCommand, string>
    {
        public async Task<string> Handle(InsertUserCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid(out string errorMessage))
                throw new BadRequestException(errorMessage);

            bool userExists = await managersService.UserExistsAsync(
                request.UserName,
                request.Email,
                cancellationToken);

            if (userExists)
                throw new BadRequestException(Resources.Translation.UserExists);

            AppUser user = new()
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = true,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                LanguageId = request.LanguageId,
                CreatedBy = currentUserService.UserId,
                Created = dateTimeService.Now
            };

            (Result Result, string Id) result;

            result = await managersService.CreateUserAsync(user, request.Password, request.Role);

            if (!result.Result.Succeeded)
                throw new BadRequestException(string.Concat(result.Result.Errors));

            return result.Id;
        }
    }
}