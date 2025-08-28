namespace Captcha.Core.Models;

public record ThemeConfiguration
{
    public string? PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
}
