namespace CaptchaWebApi.Controllers;
using Captcha.Core.Extensions;
using Captcha.Core.Models;
using Captcha.Core.Services;
using Examples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiController]
[Route("[controller]")]
public class CaptchaController(ICaptchaService captchaService) : Controller
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(CaptchaRequest), typeof(CreateCaptchaExamples))]
    public async Task<FileContentResult> Create(CaptchaRequest request)
    {
        var domain = request.ToDomain();

        return await captchaService.CreateCaptchaImageAsync(domain);
    }
}
