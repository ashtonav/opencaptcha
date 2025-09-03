namespace Captcha.Core.Models;

using SkiaSharp;

public record CaptchaConfigurationData
{
    public required string Text { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required float Frequency { get; set; }
    public required SKColor PrimaryColor { get; init; }
    public required SKColor SecondaryColor { get; init; }
}
