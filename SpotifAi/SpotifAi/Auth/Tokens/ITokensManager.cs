using System.Security.Claims;

namespace SpotifAi.Auth.Tokens;

public interface ITokensManager
{
    JsonWebToken CreateToken(Guid userId, List<string> roles, List<Claim>? claims = null);
}