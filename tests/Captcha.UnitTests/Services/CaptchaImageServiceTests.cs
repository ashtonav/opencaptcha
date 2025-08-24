namespace Captcha.UnitTests.Services;

using System.Drawing;
using System.Drawing.Imaging;
using Captcha.Core.Models;
using Captcha.Core.Services;
using NUnit.Framework;

[TestFixture]
public class CaptchaImageTests
{
    private readonly CaptchaImageService _service = new();

    [Test]
    public void CaptchaImageWithEmptyTextCreatesImage()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = string.Empty,
            Width = 150,
            Height = 60,
            Font = "Arial Unicode MS",
            Frequency = 5,
            PrimaryColor = Color.Red,
            SecondaryColor = Color.Green,
            ThirdColor = Color.Blue
        };

        // Act
        using var bitmap = _service.CreateCaptchaImage(config);

        // Assert
        Assert.That(bitmap, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bitmap.Width, Is.EqualTo(150));
            Assert.That(bitmap.Height, Is.EqualTo(60));
        }
    }

    [Test]
    public void CaptchaImageWithHighFrequencyCreatesImage()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "High freq",
            Width = 200,
            Height = 80,
            Font = "Arial Unicode MS",
            Frequency = 100000,
            PrimaryColor = Color.Red,
            SecondaryColor = Color.Green,
            ThirdColor = Color.Blue
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using var bmp = _service.CreateCaptchaImage(config);
            Assert.That(bmp, Is.Not.Null);
        });
    }

    [Test]
    public void CaptchaImageCreatesImageWithCorrectPixelFormat()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Pixel Format Test",
            Width = 100,
            Height = 50,
            Font = "Arial Unicode MS",
            Frequency = 3,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.LightGray,
            ThirdColor = Color.Gray
        };

        // Act
        using var bmp = _service.CreateCaptchaImage(config);

        // Assert
        Assert.That(bmp.PixelFormat, Is.EqualTo(PixelFormat.Format16bppRgb555));
    }

    [Test]
    public void CaptchaImageWithAlphaChannelColorsNoErrors()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Alpha",
            Width = 120,
            Height = 40,
            Font = "Arial Unicode MS",
            Frequency = 5,
            PrimaryColor = Color.FromArgb(128, Color.Red),
            SecondaryColor = Color.FromArgb(128, Color.Green),
            ThirdColor = Color.FromArgb(128, Color.Blue)
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using var bitmap = _service.CreateCaptchaImage(config);
            Assert.That(bitmap, Is.Not.Null);
        });
    }

    [Test]
    public void CaptchaImageWithExtremelyLargeText()
    {
        // Arrange
        var longText = new string('W', 200);
        var config = new CaptchaConfigurationData
        {
            Text = longText,
            Width = 300,
            Height = 100,
            Font = "Arial Unicode MS",
            Frequency = 5,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            ThirdColor = Color.Gray
        };

        // Act
        using var image = _service.CreateCaptchaImage(config);

        // Assert
        Assert.That(image, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(image.Width, Is.EqualTo(300));
            Assert.That(image.Height, Is.EqualTo(100));
        }
    }

    [Test]
    public void CaptchaImageWithNullConfigThrowsNullException()
    {
        // Arrange
        CaptchaConfigurationData nullConfig = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() =>
        {
            _service.CreateCaptchaImage(nullConfig);
        });
    }

    [Test]
    public void CaptchaImageWithWhitespaceOnlyTextCreatesValidImage()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "   ", // only whitespace
            Width = 100,
            Height = 50,
            Font = "Arial Unicode MS",
            Frequency = 4,
            PrimaryColor = Color.LightGray,
            SecondaryColor = Color.DarkGray,
            ThirdColor = Color.Gray
        };

        // Act
        using var bitmap = _service.CreateCaptchaImage(config);

        // Assert
        Assert.That(bitmap, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bitmap.Width, Is.EqualTo(100));
            Assert.That(bitmap.Height, Is.EqualTo(50));
        }
    }

    [Test]
    public void TwoCaptchaImagesAreNotTheSame()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "WarpTest",
            Width = 120,
            Height = 50,
            Font = "Arial Unicode MS",
            Frequency = 4,
            PrimaryColor = Color.Blue,
            SecondaryColor = Color.Red,
            ThirdColor = Color.Green
        };


        // Act
        using var bmp1 = _service.CreateCaptchaImage(config);
        using var bmp2 = _service.CreateCaptchaImage(config);

        byte[] data1, data2;
        using (var ms1 = new MemoryStream())
        using (var ms2 = new MemoryStream())
        {
            bmp1.Save(ms1, ImageFormat.Png);
            bmp2.Save(ms2, ImageFormat.Png);
            data1 = ms1.ToArray();
            data2 = ms2.ToArray();
        }

        // Assert
        Assert.That(data1, Is.Not.EqualTo(data2),
            "Expected images to differ due to random text warping, but they appear identical.");
    }

    [Test]
    public void CaptchaImageBackgroundIsFilledWithTertiaryColorCombination()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "BackgroundCheck",
            Width = 100,
            Height = 40,
            Font = "Arial Unicode MS",
            Frequency = 2,
            PrimaryColor = Color.Yellow,
            SecondaryColor = Color.Red,
            ThirdColor = Color.Blue
        };

        // Act
        using var bitmap = _service.CreateCaptchaImage(config);
        var foundMixedColor = false;

        for (var x = 0; x < bitmap.Width; x += 5)
        {
            for (var y = 0; y < bitmap.Height; y += 5)
            {
                var pixel = bitmap.GetPixel(x, y);
                if (pixel != Color.Red && pixel != Color.Blue && pixel != Color.Yellow)
                {
                    foundMixedColor = true;
                    break;
                }
            }
            if (foundMixedColor)
            {
                break;
            }
        }

        // Assert
        Assert.That(foundMixedColor, Is.True,
            "Expected to find a 'mixed' or different color in the background due to the hatch brush combination.");
    }

    [Test]
    public void CaptchaImageCreateDoesNotChangeRequestObject()
    {
        // Arrange
        var originalConfig = new CaptchaConfigurationData
        {
            Text = "ImmutableCheck",
            Width = 150,
            Height = 60,
            Font = "Arial Unicode MS",
            Frequency = 10,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            ThirdColor = Color.Gray
        };

        var configCopy = new CaptchaConfigurationData
        {
            Text = originalConfig.Text,
            Width = originalConfig.Width,
            Height = originalConfig.Height,
            Font = originalConfig.Font,
            Frequency = originalConfig.Frequency,
            PrimaryColor = originalConfig.PrimaryColor,
            SecondaryColor = originalConfig.SecondaryColor,
            ThirdColor = originalConfig.ThirdColor
        };

        // Act
        _ = _service.CreateCaptchaImage(originalConfig);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(originalConfig.Text, Is.EqualTo(configCopy.Text), "Text should remain unchanged.");
            Assert.That(originalConfig.Width, Is.EqualTo(configCopy.Width), "Width should remain unchanged.");
            Assert.That(originalConfig.Height, Is.EqualTo(configCopy.Height), "Height should remain unchanged.");
            Assert.That(originalConfig.Font, Is.EqualTo(configCopy.Font), "Font should remain unchanged.");
            Assert.That(originalConfig.Frequency, Is.EqualTo(configCopy.Frequency), "Frequency should remain unchanged.");
            Assert.That(originalConfig.PrimaryColor, Is.EqualTo(configCopy.PrimaryColor), "PrimaryColor should remain unchanged.");
            Assert.That(originalConfig.SecondaryColor, Is.EqualTo(configCopy.SecondaryColor), "SecondaryColor should remain unchanged.");
            Assert.That(originalConfig.ThirdColor, Is.EqualTo(configCopy.ThirdColor), "TertiaryColor should remain unchanged.");
        }
    }

    [Test]
    public void CaptchaImageWithSpecialCharacters()
    {
        // Arrange
        var specialText = "!@#$%^&*()_+中文测试—🚀";
        var config = new CaptchaConfigurationData
        {
            Text = specialText,
            Width = 250,
            Height = 100,
            Font = "Arial Unicode MS",
            Frequency = 6,
            PrimaryColor = Color.Brown,
            SecondaryColor = Color.Beige,
            ThirdColor = Color.Pink
        };

        // Act
        using var bmp = _service.CreateCaptchaImage(config);

        // Assert
        Assert.That(bmp, Is.Not.Null, "Bitmap should not be null for text with special characters.");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bmp.Width, Is.EqualTo(250));
            Assert.That(bmp.Height, Is.EqualTo(100));
        }
    }

    [Test]
    public void CreateCaptchaWith1CharacterText()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "W",
            Width = 150,
            Height = 50,
            Font = "Arial Unicode MS",
            Frequency = 5,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.LightGray,
            ThirdColor = Color.Gray
        };

        // Act
        using var bmp = _service.CreateCaptchaImage(config);

        // Assert
        Assert.That(bmp, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bmp.Width, Is.EqualTo(150));
            Assert.That(bmp.Height, Is.EqualTo(50));
        }
    }
}
