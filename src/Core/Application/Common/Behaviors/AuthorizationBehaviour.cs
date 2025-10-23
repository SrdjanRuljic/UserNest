using Application.Common.Interfaces;
using Application.Common.Security;
using Application.Exceptions;
using MediatR;
using System.Reflection;

namespace Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse>(ICurrentUserService currentUserService, IManagersService managersService)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            IEnumerable<AuthorizeAttribute> authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if (authorizeAttributes.Any())
            {
                if (currentUserService.UserId == null)
                    throw new UnauthorizedAccessException(ErrorMessages.Unauthorized);

                IEnumerable<AuthorizeAttribute> authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

                if (authorizeAttributesWithRoles.Any())
                {
                    bool authorized = false;

                    foreach (string[] roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        foreach (string role in roles)
                        {
                            bool isInRole = await managersService.IsInRoleAsync(currentUserService.UserId, role.Trim());
                            if (isInRole)
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }

                    if (!authorized)
                        throw new ForbiddenAccessException(ErrorMessages.Forbidden);
                }

                IEnumerable<AuthorizeAttribute> authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

                if (authorizeAttributesWithPolicies.Any())
                {
                    foreach (string policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                    {
                        bool authorized = await managersService.AuthorizeAsync(currentUserService.UserId, policy, cancellationToken);

                        if (!authorized)
                        {
                            throw new ForbiddenAccessException(ErrorMessages.Forbidden);
                        }
                    }
                }
            }

            return await next(cancellationToken);
        }
    }
}