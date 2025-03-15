namespace Captcha.UnitTests.Services;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Captcha.Core.Models;
using Captcha.Core.Services;
using NUnit.Framework;

[TestFixture]
public class CaptchaServiceTests
{
    [Test]
    public void CreateCaptchaImageAsyncWhenConfigIsNullThrowsArgumentNullException()
    {
        // Arrange
        CaptchaConfigurationData config = null;
        var captchaService = new CaptchaService();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await captchaService.CreateCaptchaImageAsync(config),
            "Expected an ArgumentNullException when config is null."
        );
    }

    [Test]
    public void CreateCaptchaImageAsyncWhenWidthIsZeroThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Test",
            Width = 0,
            Height = 100,
            Frequency = 10F,
            Font = "Arial",
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.White
        };
        var captchaService = new CaptchaService();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            async () => await captchaService.CreateCaptchaImageAsync(config));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex.ParamName, Is.EqualTo("config"));
            Assert.That(ex.ActualValue, Is.EqualTo(0));
        }
    }

    [Test]
    public void CreateCaptchaImageAsyncWhenWidthIsOver1000ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Test",
            Width = 1025,
            Height = 100,
            Frequency = 10F,
            Font = "Arial",
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.White
        };
        var captchaService = new CaptchaService();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            async () => await captchaService.CreateCaptchaImageAsync(config));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex.ParamName, Is.EqualTo("config"));
            Assert.That(ex.ActualValue, Is.EqualTo(1025));
        }
    }

    [Test]
    public void CreateCaptchaImageAsyncWhenHeightIsZeroThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Test",
            Width = 100,
            Height = 0,
            Frequency = 10F,
            Font = "Arial",
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.White
        };
        var captchaService = new CaptchaService();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            async () => await captchaService.CreateCaptchaImageAsync(config));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex.ParamName, Is.EqualTo("config"));
            Assert.That(ex.ActualValue, Is.EqualTo(0));
        }
    }

    [Test]
    public void CreateCaptchaImageAsyncWhenHeightIsOver1000ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Test",
            Width = 200,
            Height = 1025,
            Frequency = 10F,
            Font = "Arial",
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.White
        };
        var captchaService = new CaptchaService();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            async () => await captchaService.CreateCaptchaImageAsync(config));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex.ParamName, Is.EqualTo("config"));
            Assert.That(ex.ActualValue, Is.EqualTo(1025));
        }
    }

    [Test]
    public async Task CreateCaptchaImageAsyncReturnedFileContentsContainsJpegSignature()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "my text",
            Width = 100,
            Height = 100,
            Frequency = 5F,
            Font = "Arial",
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.White
        };
        var captchaService = new CaptchaService();

        // Act
        var result = await captchaService.CreateCaptchaImageAsync(config);

        // Assert
        Assert.That(result.FileContents, Is.Not.Empty, "FileContents should not be empty.");
        using (Assert.EnterMultipleScope())
        {
            // Check for JPEG magic bytes: 0xFF, 0xD8 at the start
            Assert.That(result.FileContents[0], Is.EqualTo(0xFF));
            Assert.That(result.FileContents[1], Is.EqualTo(0xD8));
            // And typically 0xFF, 0xD9 near the end — we’ll just check the final two bytes
            Assert.That(result.FileContents[^2], Is.EqualTo(0xFF));
            Assert.That(result.FileContents[^1], Is.EqualTo(0xD9));
        }
    }

    [Test]
    public async Task CreateCaptchaImageAsyncFileContentResultBinaryIsValidImage()
    {
        // Arrange
        // We'll attempt to load the returned bytes into a Bitmap to confirm it's a valid image.
        var config = new CaptchaConfigurationData
        {
            Text = "LoadImageCheck",
            Width = 120,
            Height = 60,
            Frequency = 5F,
            Font = "Arial",
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.White
        };
        var captchaService = new CaptchaService();

        // Act
        var result = await captchaService.CreateCaptchaImageAsync(config);

        // Assert
        using var memoryStream = new MemoryStream(result.FileContents);
        using var bitmap = Image.FromStream(memoryStream); // Should not throw if the data is a valid image
        Assert.That(bitmap, Is.Not.Null, "Image.FromStream should produce a valid Bitmap object.");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bitmap.Width, Is.EqualTo(120));
            Assert.That(bitmap.Height, Is.EqualTo(60));
            // Optional: Check that the loaded image is a JPEG
            Assert.That(bitmap.RawFormat, Is.EqualTo(ImageFormat.Jpeg));
        }
    }

    [Test]
    public async Task CreateCaptchaImageAsyncDoesNotMutateConfig()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Immutable Check",
            Width = 100,
            Height = 100,
            Frequency = 10F,
            Font = "Arial",
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.White
        };

        var configCopy = new CaptchaConfigurationData
        {
            Text = config.Text,
            Width = config.Width,
            Height = config.Height,
            Frequency = config.Frequency,
            Font = config.Font,
            PrimaryColor = config.PrimaryColor,
            SecondaryColor = config.SecondaryColor,
            TertiaryColor = config.TertiaryColor
        };

        var captchaService = new CaptchaService();

        // Act
        await captchaService.CreateCaptchaImageAsync(config);

        // Assert
        // Confirm the original config object didn't get changed by the service call.
        Assert.Multiple(() =>
        {
            Assert.That(config.Text, Is.EqualTo(configCopy.Text));
            Assert.That(config.Width, Is.EqualTo(configCopy.Width));
            Assert.That(config.Height, Is.EqualTo(configCopy.Height));
            Assert.That(config.Frequency, Is.EqualTo(configCopy.Frequency));
            Assert.That(config.Font, Is.EqualTo(configCopy.Font));
            Assert.That(config.PrimaryColor, Is.EqualTo(configCopy.PrimaryColor));
            Assert.That(config.SecondaryColor, Is.EqualTo(configCopy.SecondaryColor));
            Assert.That(config.TertiaryColor, Is.EqualTo(configCopy.TertiaryColor));
        });
    }

    [Test]
    public async Task CreateCaptchaImageAsyncEmptyTextDoesNotThrowAndReturnsImage()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = string.Empty,
            Width = 100,
            Height = 40,
            Frequency = 10F,
            Font = "Arial",
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            TertiaryColor = Color.Gray
        };

        var service = new CaptchaService();

        // Act & Assert
        // Even with empty text, we still expect a valid image result—no exception.
        var result = await service.CreateCaptchaImageAsync(config);
        Assert.That(result, Is.Not.Null, "Expected a FileContentResult even when text is empty.");
        Assert.That(result.FileContents, Is.Not.Empty, "FileContents should not be empty.");
    }

    [Test]
    public async Task CreateCaptchaImageAsyncCanBeCalledRepeatedlyWithSameConfigProducesNonEmptyResults()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Repeated calls",
            Width = 150,
            Height = 60,
            Frequency = 5F,
            Font = "Arial",
            PrimaryColor = Color.Green,
            SecondaryColor = Color.LightGreen,
            TertiaryColor = Color.White
        };
        var service = new CaptchaService();

        // Act
        var result1 = await service.CreateCaptchaImageAsync(config);
        var result2 = await service.CreateCaptchaImageAsync(config);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.Not.Null, "First call should produce a valid FileContentResult.");
            Assert.That(result1.FileContents, Is.Not.Empty, "First result bytes should be non-empty.");
            Assert.That(result2, Is.Not.Null, "Second call should produce a valid FileContentResult.");
            Assert.That(result2.FileContents, Is.Not.Empty, "Second result bytes should be non-empty.");
            // They may or may not be identical due to the random generation in CaptchaImage.
        });
    }

    [Test]
    public async Task CreateCaptchaImageAsyncAlphaChannelColorsProducesJpegSuccessfully()
    {
        // Arrange
        // Some GDI+ operations can behave oddly with alpha channels, but we should still get a valid JPEG.
        var config = new CaptchaConfigurationData
        {
            Text = "Alpha test",
            Width = 120,
            Height = 50,
            Frequency = 5F,
            Font = "Arial",
            PrimaryColor = Color.FromArgb(128, 255, 0, 0),  // semi-transparent red
            SecondaryColor = Color.FromArgb(128, 0, 255, 0),// semi-transparent green
            TertiaryColor = Color.FromArgb(128, 0, 0, 255)  // semi-transparent blue
        };

        var service = new CaptchaService();

        // Act
        var result = await service.CreateCaptchaImageAsync(config);

        // Assert
        Assert.That(result.FileContents, Is.Not.Null.And.Not.Empty);
        using (Assert.EnterMultipleScope())
        {
            // Quick check for JPEG signature:
            Assert.That(result.FileContents[0], Is.EqualTo(0xFF));
            Assert.That(result.FileContents[1], Is.EqualTo(0xD8));
            Assert.That(result.FileContents[^2], Is.EqualTo(0xFF));
            Assert.That(result.FileContents[^1], Is.EqualTo(0xD9));
        }
    }

    [Test]
    public async Task CreateCaptchaImageAsyncConcurrentCallsDoNotThrowOrCorruptResults()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Thread test",
            Width = 100,
            Height = 40,
            Frequency = 5F,
            Font = "Arial",
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            TertiaryColor = Color.Gray
        };
        var service = new CaptchaService();

        var exceptions = new List<Exception>();
        var lockObj = new object();

        // Act
        await Task.WhenAll(
            Enumerable.Range(0, 20).Select(async _ =>
            {
                try
                {
                    var result = await service.CreateCaptchaImageAsync(config);
                    // Simple check
                    if (result.FileContents == null || result.FileContents.Length == 0)
                    {
                        throw new InvalidOperationException("Empty result in concurrent call");
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObj)
                    {
                        exceptions.Add(ex);
                    }
                }
            })
        );

        // Assert
        // We want zero exceptions after running concurrency tests.
        Assert.That(exceptions, Is.Empty,
            "Expected no exceptions from concurrent CreateCaptchaImageAsync calls, but found some.");
    }
}
