namespace Captcha.Core.Models;

public record GetCreateCaptchaRequest
{
    public required string Text { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public CaptchaDifficulty? Difficulty { get; init; }
    public ThemeConfiguration? Theme { get; set; }
}
