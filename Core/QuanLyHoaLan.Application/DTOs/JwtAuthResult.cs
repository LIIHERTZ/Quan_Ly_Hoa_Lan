namespace QuanLyHoaLan.Application.DTOs;

public class JwtAuthResult
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
