using Domain.Entities.Identity;

namespace Application.Common.Behaviors
{
    internal static class FullNameResolver
    {
        public static string Resolve(AppUser source)
        {
            string fullName = string.Empty;

            if (source == null)
                return fullName;

            if (string.IsNullOrEmpty(source.FirstName) && string.IsNullOrEmpty(source.LastName))
                fullName = source.Email!;
            else if (!string.IsNullOrEmpty(source.FirstName) && !string.IsNullOrEmpty(source.LastName))
                fullName = source.FirstName + " " + source.LastName;
            else if (!string.IsNullOrEmpty(source.FirstName) && string.IsNullOrEmpty(source.LastName))
                fullName = source.FirstName;
            else if (string.IsNullOrEmpty(source.FirstName) && !string.IsNullOrEmpty(source.LastName))
                fullName = source.LastName;

            return fullName;
        }
    }
}