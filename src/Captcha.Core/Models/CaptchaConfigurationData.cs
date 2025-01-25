namespace Captcha.Core.Models;

public record CaptchaConfigurationData
{
    public required string Text { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required string Font { get; init; }
    public required CaptchaDifficulty Difficulty { get; init; }
}
