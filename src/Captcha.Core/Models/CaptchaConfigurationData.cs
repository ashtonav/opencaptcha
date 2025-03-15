namespace Captcha.Core.Models;

using System.Drawing;

public record CaptchaConfigurationData
{
    public required string Text { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required string Font { get; init; }
    public required float Frequency { get; set; }
    public required Color PrimaryColor { get; init; }
    public required Color SecondaryColor { get; init; }
    public required Color ThirdColor { get; init; }
}
