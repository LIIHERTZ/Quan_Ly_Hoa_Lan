using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using QuanLyHoaLan.Application.Interfaces;

namespace QuanLyHoaLan.Infrastructure.Authentication;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;

    public GoogleAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<GoogleUserInfo?> ValidateAsync(string idToken)
    {
        try
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            
            return new GoogleUserInfo
            {
                Email = payload.Email,
                Name = payload.Name,
                GoogleId = payload.Subject,
                Picture = payload.Picture
            };
        }
        catch
        {
            // Invalid token or expired
            return null;
        }
    }
}
