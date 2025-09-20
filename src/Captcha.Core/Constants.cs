namespace Captcha.Core;

using SkiaSharp;

public static class Constants
{
    public const int MinCaptchaSize = 10;
    public const int MaxCaptchaSize = 1024;
    public const int DefaultCaptchaWidth = 400;
    public const int DefaultCaptchaHeight = 100;
    public const float DefaultFrequency = 100F;
    public const int FrequencyScalingFactor = 40000; // on a 400 x 100 image
    public static readonly SKColor DefaultPrimaryColor = SKColor.Parse("FFD3D3D3");
    public static readonly SKColor DefaultSecondaryColor = SKColor.Parse("FFFFFFFF");
    public const string CaptchaContentType = "image/jpeg";
    public const float WarpCaptchaTextFrequency = 4F;
    public const int CaptchaNoise = 50;
    public static SKTypeface MainFontTypeface { get; } =
        SKTypeface.FromStream(typeof(Constants).Assembly.GetManifestResourceStream("Captcha.Core.Resources.Fonts.Caveat-SemiBold.ttf"));
    public static SKTypeface FallbackFontTypeface { get; } =
        SKTypeface.FromStream(typeof(Constants).Assembly.GetManifestResourceStream("Captcha.Core.Resources.Fonts.Unifont.ttf"));
}
