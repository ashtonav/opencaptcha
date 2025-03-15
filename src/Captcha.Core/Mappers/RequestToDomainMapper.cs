namespace Captcha.Core.Mappers;

using Models;
using static Models.CaptchaDifficulty;

public class RequestToDomainMapper
{
    public CaptchaConfigurationData ToDomain(CaptchaRequest request) => new()
    {
        Text = request.Text,
        Width = request.Width ?? Constants.DefaultCaptchaWidth,
        Height = request.Height ?? Constants.DefaultCaptchaHeight,
        Frequency = GetFrequency(request.Difficulty),
        Font = Constants.DefaultCaptchaFontName,
        PrimaryColor = Constants.DefaultPrimaryColor,
        SecondaryColor = Constants.DefaultSecondaryColor,
        ThirdColor = Constants.DefaultTertiaryColor,
    };

    private static float GetFrequency(CaptchaDifficulty? difficulty) => difficulty switch
    {
        Easy => 300F,
        Challenging => 30F,
        Hard => 20F,
        Medium or _ => Constants.DefaultFrequency
    };
}
