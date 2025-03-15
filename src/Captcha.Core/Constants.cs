namespace Captcha.Core;

using System.Drawing;
using System.Drawing.Imaging;

public static class Constants
{
    public const int MinCaptchaSize = 10;
    public const int MaxCaptchaSize = 1024;
    public const int DefaultCaptchaWidth = 400;
    public const int DefaultCaptchaHeight = 100;
    public const float DefaultFrequency = 100F;
    public const string DefaultCaptchaFontName = "Arial Unicode MS";
    public static readonly Color DefaultPrimaryColor = Color.DarkGray;
    public static readonly Color DefaultSecondaryColor = Color.LightGray;
    public static readonly Color DefaultTertiaryColor = Color.White;
    public const string CaptchaContentType = "image/jpeg";
    public static readonly ImageFormat CaptchaImageFormat = ImageFormat.Jpeg;
    public const float WarpCaptchaTextFrequency = 4F;
    public const int CaptchaNoise = 50;
}
