namespace Captcha.Core.Services;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc;
using Models;

public class CaptchaService : ICaptchaService
{
    public async Task<FileContentResult> CreateCaptchaImageAsync(CaptchaConfigurationData config)
    {
        ValidateRequest(config);

        var image = new CaptchaImage(config);
        using var created = image.Create();

        // Save the image to a memory stream so we can return it as a file
        await using var memoryStream = new MemoryStream();
        created.Save(memoryStream, ImageFormat.Jpeg);

        // Return the image as a jpeg file
        return new FileContentResult(memoryStream.ToArray(), "image/jpeg");
    }

    private static void ValidateRequest(CaptchaConfigurationData config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (config.Width is <= Constants.MinCaptchaSize or > Constants.MaxCaptchaSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(config),
                config.Width,
                $"Width must be between {Constants.MinCaptchaSize} and {Constants.MaxCaptchaSize} inclusive."
            );
        }

        if (config.Height is <= Constants.MinCaptchaSize or > Constants.MaxCaptchaSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(config),
                config.Height,
                $"Height must be between {Constants.MinCaptchaSize} and {Constants.MaxCaptchaSize} inclusive."
            );
        }
    }

}
