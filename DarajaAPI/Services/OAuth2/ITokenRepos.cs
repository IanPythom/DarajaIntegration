using Microsoft.AspNetCore.Identity;

namespace MailServiceAPI.Services.OAuth2
{
    public interface ITokenRepos
    {
        public string GenerateJWT(IdentityUser user, List<string> roles);
    }
}