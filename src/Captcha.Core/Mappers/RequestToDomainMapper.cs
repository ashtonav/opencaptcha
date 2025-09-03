namespace Captcha.Core.Mappers;

using Models;
using static Models.CaptchaDifficulty;

public class RequestToDomainMapper
{
    public CaptchaConfigurationData ToDomain(CaptchaRequest request)
    {
        var width = request.Width ?? Constants.DefaultCaptchaWidth;
        var height = request.Height ?? Constants.DefaultCaptchaHeight;

        return new CaptchaConfigurationData
        {
            Text = request.Text,
            Width = width,
            Height = height,
            Frequency = GetFrequency(request.Difficulty, width, height),
            PrimaryColor = Constants.DefaultPrimaryColor,
            SecondaryColor = Constants.DefaultSecondaryColor,
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
}
