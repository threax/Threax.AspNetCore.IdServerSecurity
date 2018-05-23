using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Threax.AspNetCore.JwtCookieAuth
{
    public interface ITokenValidationParametersProvider
    {
        Task<TokenValidationParameters> GetTokenValidationParameters(JwtCookieAuthenticationOptions options);
    }
}