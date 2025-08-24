namespace Captcha.Core.Services;

using System.Drawing;
using Models;

public interface ICaptchaImageService
{
    public Bitmap CreateCaptchaImage(CaptchaConfigurationData config);
}
