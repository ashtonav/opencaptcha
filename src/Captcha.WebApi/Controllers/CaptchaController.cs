namespace Captcha.WebApi.Controllers;

using Captcha.Core;
using Captcha.Core.Mappers;
using Captcha.Core.Models;
using Captcha.Core.Services;
using Examples;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using Swashbuckle.AspNetCore.Filters;

[ApiController]
[Route("[controller]")]
public class CaptchaController(ICaptchaImageService captchaImageService, RequestToDomainMapper requestToDomainMapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<FileContentResult> GetCreateAsync([FromQuery] GetCreateCaptchaRequest request)
    {
        var domain = requestToDomainMapper.ToDomain(request);

        using var created = captchaImageService.CreateCaptchaImage(domain);

        return await SerializeToJpegFile(created);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(PostCreateCaptchaRequest), typeof(CreateCaptchaExamples))]
    public async Task<FileContentResult> PostCreateAsync(PostCreateCaptchaRequest request)
    {
        var domain = requestToDomainMapper.ToDomain(request);

        using var created = captchaImageService.CreateCaptchaImage(domain);

        return await SerializeToJpegFile(created);
    }

    private async Task<FileContentResult> SerializeToJpegFile(SKBitmap image)
    {
        await using var memoryStream = new MemoryStream();
        SKImage.FromBitmap(image)
            .Encode(SKEncodedImageFormat.Jpeg, 100)
            .SaveTo(memoryStream);

        return new FileContentResult(memoryStream.ToArray(), Constants.CaptchaContentType);
    }
}
