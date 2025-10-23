namespace Application.Common.Interfaces
{
    public interface IJwtFactory
    {
        string GenerateEncodedToken();

        string GenerateEncodedToken(string userId, string[] roles);

        bool Validate(string refreshToken);
    }
}