namespace QuanLyHoaLan.Application.Interfaces;

public class GoogleUserInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GoogleId { get; set; } = string.Empty;
    public string? Picture { get; set; }
}

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> ValidateAsync(string idToken);
}
