namespace CaptchaWebApi.Controllers.Examples;
using Captcha.Core.Models;
using Swashbuckle.AspNetCore.Filters;

public record CreateCaptchaExamples : IMultipleExamplesProvider<CaptchaRequest>
{
    public IEnumerable<SwaggerExample<CaptchaRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Example 1",
            "Example 1 - Create captcha with text",
            new CaptchaRequest
            {
                Text = "hello"
            });

        yield return SwaggerExample.Create(
            "Example 2",
            "Example 2 - Create challenging captcha with text",
            new CaptchaRequest
            {
                Text = "hello",
                Difficulty = CaptchaDifficulty.Challenging
            });

        yield return SwaggerExample.Create(
            "Example 3",
            "Example 3 - Create hard captcha with text",
            new CaptchaRequest
            {
                Text = "hello",
                Difficulty = CaptchaDifficulty.Hard
            });

        yield return SwaggerExample.Create(
            "Example 4",
            "Example 4 - Create captcha with text and height and width",
            new CaptchaRequest
            {
                Text = "world",
                Height = 300,
                Width = 300
            });
    }
}
