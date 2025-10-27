using Application.Common.Security;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Users.Queries.ValidatePassword
{
    [Authorize(Policy = nameof(Policies.RequireAuthorization))]
    public class ValidatePasswordQuery : IRequest<bool>
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}