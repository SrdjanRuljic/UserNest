using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth
{
    public class JwtFactory(IConfiguration configuration, IDateTimeService dateTimeService) : IJwtFactory
    {
        public string GenerateEncodedToken(string userId, string[] roles)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Jwt:AccessTokenSecret"]!));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha512Signature);

            DateTime now = dateTimeService.Now;
            DateTime expires = now.AddHours(8);

            Claim[] GetClaims()
            {
                List<Claim> claims =
                [
                    new Claim("sub", userId),
                    new Claim("jti", Guid.NewGuid().ToString()),
                ];

                foreach (var item in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item));
                }

                return [.. claims];
            }

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(GetClaims()),
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!,
                Expires = expires,
                NotBefore = now,
                SigningCredentials = credentials
            };

            JsonWebTokenHandler tokenHandler = new();
            string encodedToken = tokenHandler.CreateToken(tokenDescriptor);

            return encodedToken;
        }

        public string GenerateEncodedToken()
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Jwt:RefreshTokenSecret"]!));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha512Signature);

            DateTime now = dateTimeService.Now;
            DateTime expires = now.AddHours(12);

            static Claim[] GetClaims()
            {
                List<Claim> claims = [new Claim("jti", Guid.NewGuid().ToString())];

                return [.. claims];
            }

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(GetClaims()),
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!,
                Expires = expires,
                NotBefore = now,
                SigningCredentials = credentials
            };

            JsonWebTokenHandler tokenHandler = new();
            string encodedToken = tokenHandler.CreateToken(tokenDescriptor);

            return encodedToken;
        }

        public async Task<bool> ValidateAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            JsonWebTokenHandler tokenHandler = new();

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:RefreshTokenSecret"]!)),
                ClockSkew = TimeSpan.Zero
            };

            TokenValidationResult result = await tokenHandler.ValidateTokenAsync(refreshToken, validationParameters);

            return result.IsValid;
        }
    }
}