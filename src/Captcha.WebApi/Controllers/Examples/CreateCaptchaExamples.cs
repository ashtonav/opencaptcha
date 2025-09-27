namespace Captcha.WebApi.Controllers.Examples;

using Captcha.Core.Models;
using Swashbuckle.AspNetCore.Filters;

public record CreateCaptchaExamples : IMultipleExamplesProvider<PostCreateCaptchaRequest>
{
    public IEnumerable<SwaggerExample<PostCreateCaptchaRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Example 1",
            "Example 1 - Create captcha with text",
            new PostCreateCaptchaRequest
            {
                Text = "hello world"
            });

        yield return SwaggerExample.Create(
            "Example 2",
            "Example 2 - Create challenging captcha with text",
            new PostCreateCaptchaRequest
            {
                Text = "hello world",
                Difficulty = CaptchaDifficulty.Challenging
            });

        yield return SwaggerExample.Create(
            "Example 3",
            "Example 3 - Create hard captcha with text",
            new PostCreateCaptchaRequest
            {
                Text = "hello world",
                Difficulty = CaptchaDifficulty.Hard
            });

        yield return SwaggerExample.Create(
            "Example 4",
            "Example 4 - Create captcha with text and height and width",
            new PostCreateCaptchaRequest
            {
                Text = "world",
                Height = 300,
                Width = 300
            });

        yield return SwaggerExample.Create(
            "Example 5",
            "Example 5 - Create captcha with a color theme",
            new PostCreateCaptchaRequest
            {
                Text = "hello world",
                Theme = new ThemeConfiguration
                {
                    PrimaryColor = "#ADD8E6",
                    SecondaryColor = "#FFFFFF",
                }
            });
    }
}
