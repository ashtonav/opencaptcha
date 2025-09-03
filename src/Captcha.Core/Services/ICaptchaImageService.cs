namespace Captcha.Core.Services;

using Models;
using SkiaSharp;

public interface ICaptchaImageService
{
    public SKBitmap CreateCaptchaImage(CaptchaConfigurationData config);
}
