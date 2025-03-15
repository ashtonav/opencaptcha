namespace Captcha.Core.Extensions;

using System.Drawing;
using Models;
using static Models.CaptchaDifficulty;

public static class MappingExtensions
{
    private static readonly ColorConverter Converter = new();
    public static CaptchaConfigurationData ToDomain(this CaptchaRequest request) => new()
    {
        Text = request.Text,
        Width = request.Width ?? Constants.DefaultCaptchaWidth,
        Height = request.Height ?? Constants.DefaultCaptchaHeight,
        Frequency = GetFrequency(request.Difficulty),
        Font = Constants.DefaultCaptchaFontName,
        PrimaryColor = GetColorFromHex(request.Colors?.Primary) ?? Constants.DefaultPrimaryColor,
        SecondaryColor = GetColorFromHex(request.Colors?.Secondary) ?? Constants.DefaultSecondaryColor,
        TertiaryColor = GetColorFromHex(request.Colors?.Tertiary) ?? Constants.DefaultThirdColor,
    };

    private static Color? GetColorFromHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return null;
        }

        return (Color?)Converter.ConvertFromString(hex);
    }

    private static float GetFrequency(CaptchaDifficulty? difficulty) => difficulty switch
    {
        Easy => 300F,
        Challenging => 30F,
        Hard => 20F,
        Medium or _ => Constants.DefaultFrequency
    };
}
