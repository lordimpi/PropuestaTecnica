using PropuestaTecnica.Common.Entities;

namespace PropuestaTecnica.Bussiness.Services.Auth;

public interface ITokenService
{
    Task<(string accessToken, RefreshToken refreshToken)> GenerateTokens(ApplicationUser user);
}