using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PomodoroTimer.Api.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PomodoroTimer.Api.Services
{
    public class TokenGenerator
    {
        private readonly JwtSettings jwtSettings;
        private readonly UserManager<PomodoroUser> userManager;

        public TokenGenerator(IConfiguration configuration, UserManager<PomodoroUser> userManager)
        {
            this.userManager = userManager;
            jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        }

        public async Task<(string accessToken, string refreshToken)> GetTokens(PomodoroUser user)
        {
            string accessToken = await GenerateAccessToken(user);
            string refreshToken = GenerateRefreshToken(user.Id);

            return (accessToken, refreshToken);
        }

        public ClaimsPrincipal? Validate(string? token)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.RefreshTokenSecurityKey))
            };

            try
            {
                ClaimsPrincipal? claimsPrincipal = handler.ValidateToken(token, validationParameters, out SecurityToken validationToken);

                return claimsPrincipal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GenerateRefreshToken(string userId)
        {
            var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userId) };

            return GenerateToken(
                jwtSettings.Audience,
                jwtSettings.Issuer,
                jwtSettings.RefreshTokenSecurityKey,
                jwtSettings.RefreshTokenExpiryInMinutes,
                claims);
        }

        private async Task<string> GenerateAccessToken(PomodoroUser user)
        {
            var claims = await userManager.GetClaimsAsync(user);
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));

            return GenerateToken(
                jwtSettings.Audience,
                jwtSettings.Issuer,
                jwtSettings.AccessTokenSecurityKey,
                jwtSettings.AccessTokenExpiryInMinutes,
                claims);
        }

        private string GenerateToken(string audience, string issuer, string securityKey, double expiryInMinutes, IEnumerable<Claim>? claims = null)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var notBefore = DateTime.UtcNow;
            var expires = notBefore.AddMinutes(expiryInMinutes);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
