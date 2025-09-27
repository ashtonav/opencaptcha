namespace Captcha.Core.Mappers;

using Models;
using SkiaSharp;
using static Models.CaptchaDifficulty;

public class RequestToDomainMapper
{
    public CaptchaConfigurationData ToDomain(GetCreateCaptchaRequest request)
    {
        var width = request.Width ?? Constants.DefaultCaptchaWidth;
        var height = request.Height ?? Constants.DefaultCaptchaHeight;

        return new CaptchaConfigurationData
        {
            Text = request.Text,
            Width = width,
            Height = height,
            Frequency = GetFrequency(request.Difficulty, width, height),
            PrimaryColor = GetColor(request.Theme?.PrimaryColor) ?? Constants.DefaultPrimaryColor,
            SecondaryColor = GetColor(request.Theme?.SecondaryColor) ?? Constants.DefaultSecondaryColor,
        };
    }

    public CaptchaConfigurationData ToDomain(PostCreateCaptchaRequest request)
    {
        var width = request.Width ?? Constants.DefaultCaptchaWidth;
        var height = request.Height ?? Constants.DefaultCaptchaHeight;

        return new CaptchaConfigurationData
        {
            Text = request.Text,
            Width = width,
            Height = height,
            Frequency = GetFrequency(request.Difficulty, width, height),
            PrimaryColor = GetColor(request.Theme?.PrimaryColor) ?? Constants.DefaultPrimaryColor,
            SecondaryColor = GetColor(request.Theme?.SecondaryColor) ?? Constants.DefaultSecondaryColor,
        };
    }

    private static float GetFrequency(CaptchaDifficulty? difficulty, int imageWidth, int imageHeight)
    {
        var multiplier = difficulty switch
        {
            Easy => 300F,
            Challenging => 30F,
            Hard => 20F,
            Medium or _ => Constants.DefaultFrequency
        };

        var scaling = imageWidth * imageHeight;

        if (scaling < Constants.FrequencyScalingFactor)
        {
            return multiplier;
        }

        return scaling / Constants.FrequencyScalingFactor * multiplier;
    }


    private static SKColor? GetColor(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return null;
        }

        return SKColor.Parse(hex);
    }
}
