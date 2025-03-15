namespace Captcha.Core;

using System.Drawing;

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
    public static readonly Color DefaultThirdColor = Color.White;
}
