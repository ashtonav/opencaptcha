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
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(CaptchaRequest), typeof(CreateCaptchaExamples))]
    public async Task<FileContentResult> CreateAsync(CaptchaRequest request)
    {
        var domain = requestToDomainMapper.ToDomain(request);
        using var created = captchaImageService.CreateCaptchaImage(domain);

        await using var memoryStream = new MemoryStream();
        SKImage.FromBitmap(created)
            .Encode(SKEncodedImageFormat.Jpeg, 100)
            .SaveTo(memoryStream);

        return new FileContentResult(memoryStream.ToArray(), Constants.CaptchaContentType);
    }
}
