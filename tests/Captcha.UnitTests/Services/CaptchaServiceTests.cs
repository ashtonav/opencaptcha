namespace Captcha.UnitTests.Services;

using System.Threading.Tasks;
using Captcha.Core.Models;
using Captcha.Core.Services;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

[TestFixture]
public class CaptchaServiceTests
{
    [Test]
    public async Task CreateCaptchaImageAsyncReturnsFileContentResultWithCorrectContentType()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "my text",
            Width = 100,
            Height = 100,
            Difficulty = CaptchaDifficulty.Medium,
            Font = "Arial Unicode MS"
        };
        var captchaService = new CaptchaService();

        // Act
        var result = await captchaService.CreateCaptchaImageAsync(config);

        // Assert
        Assert.That(result, Is.InstanceOf<FileContentResult>(), "Result should be a FileContentResult.");
        Assert.Multiple(() =>
        {
            Assert.That(result.ContentType, Is.EqualTo("image/jpeg"), "ContentType should be image/jpeg.");
            Assert.That(result.FileContents, Is.Not.Null, "FileContents should not be null.");
        });
        Assert.That(result.FileContents, Is.Not.Empty, "FileContents should have a size greater than 0.");
    }
}
