using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Domain.Entities.Identity;
using MediatR;

namespace Application.Users.Commands.Delete
{
    internal sealed class DeleteUserCommandHandler(
        IManagersService managersService,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService) : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            AppUser? user = await managersService.FindByIdAsync(request.Id) ??
                throw new NotFoundException(nameof(AppUser), request.Id);

            user.IsDeleted = true;
            user.LastModifiedBy = currentUserService.UserId;
            user.LastModified = dateTimeService.Now;

            Result result = await managersService.UpdateUser(user);

            if (!result.Succeeded)
                throw new BadRequestException(string.Concat(result.Errors));
        }
    }
}