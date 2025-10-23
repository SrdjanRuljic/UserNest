using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth
{
    public class JwtFactory(IConfiguration config, IDateTimeService dateTimeService) : IJwtFactory
    {
        public string GenerateEncodedToken(string userId, string[] roles)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(config["Jwt:AccessTokenSecret"]!));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha512);

            Claim[] GetClaims()
            {
                List<Claim> claims =
                [
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ];

                foreach (var item in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item));
                }

                return [.. claims];
            }

            JwtSecurityToken token = new(issuer: config["Jwt:Issuer"],
                                         audience: config["Jwt:Audience"],
                                         GetClaims(),
                                         expires: dateTimeService.Now.AddHours(8),
                                         signingCredentials: credentials);

            string encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;
        }

        public string GenerateEncodedToken()
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(config["Jwt:RefreshTokenSecret"]!));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha512);

            static Claim[] GetClaims()
            {
                List<Claim> claims = [new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())];

                return [.. claims];
            }

            JwtSecurityToken token = new(issuer: config["Jwt:Issuer"],
                                         audience: config["Jwt:Audience"],
                                         GetClaims(),
                                         expires: dateTimeService.Now.AddHours(12),
                                         signingCredentials: credentials);

            string encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;
        }

        public bool Validate(string refreshToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = config["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = config["Jwt:Audience"],

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:RefreshTokenSecret"]!)),
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

            try
            {
                jwtSecurityTokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}