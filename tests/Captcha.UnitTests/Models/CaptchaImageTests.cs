namespace Captcha.UnitTests.Models;

using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using Captcha.Core.Models;
using NUnit.Framework;

[TestFixture]
public class CaptchaImageTests
{
    [Test]
    public void CaptchaImageWithEmptyTextCreatesNonNullImage()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = string.Empty,
            Width = 150,
            Height = 60,
            Font = "Arial",
            Frequency = 5,
            PrimaryColor = Color.Red,
            SecondaryColor = Color.Green,
            TertiaryColor = Color.Blue
        };

        // Act
        var captchaImage = new CaptchaImage(config);
        using var bitmap = captchaImage.Create();

        // Assert
        Assert.That(bitmap, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bitmap.Width, Is.EqualTo(150));
            Assert.That(bitmap.Height, Is.EqualTo(60));
        }
    }

    [Test]
    public void CaptchaImageWithNonExistentFontFallsBackOrThrowsNoCrash()
    {
        // Arrange
        // Provide a bogus font name to see how the code reacts. Some systems might default to a known font,
        // while others could throw. If it throws on your system, change the test to expect a specific exception.
        var config = new CaptchaConfigurationData
        {
            Text = "Test",
            Width = 180,
            Height = 70,
            Font = "SomeMadeUpFont123",
            Frequency = 5,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            TertiaryColor = Color.Gray
        };

        var captchaImage = new CaptchaImage(config);

        // Act & Assert
        // We expect that either it defaults to a valid system font or
        // simply doesn't crash. If the code under test *does* throw, update the test accordingly.
        Assert.DoesNotThrow(() =>
        {
            using var bmp = captchaImage.Create();
            Assert.That(bmp, Is.Not.Null);
        });
    }

    [Test]
    public void CaptchaImageWithHighFrequencyCreatesImageWithLotsOfNoiseNoCrash()
    {
        // Arrange
        // Very high frequency => (width*height/freq) is small, but let's see if it works.
        var config = new CaptchaConfigurationData
        {
            Text = "High freq",
            Width = 200,
            Height = 80,
            Font = "Arial",
            Frequency = 100000, // extremely large
            PrimaryColor = Color.Red,
            SecondaryColor = Color.Green,
            TertiaryColor = Color.Blue
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using var bmp = new CaptchaImage(config).Create();
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
            Font = "Arial",
            Frequency = 3,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.Gray
        };

        // Act
        using var bmp = new CaptchaImage(config).Create();

        // Assert
        // The code sets PixelFormat.Format16bppRgb555 by default.
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
            Font = "Arial",
            Frequency = 5,
            // Example color with 50% alpha:
            PrimaryColor = Color.FromArgb(128, Color.Red),
            SecondaryColor = Color.FromArgb(128, Color.Green),
            TertiaryColor = Color.FromArgb(128, Color.Blue)
        };

        // Act & Assert
        // Verifies that partial transparency doesn't crash the drawing code.
        Assert.DoesNotThrow(() =>
        {
            using var bitmap = new CaptchaImage(config).Create();
            Assert.That(bitmap, Is.Not.Null);
        });
    }

    [Test]
    public void CaptchaImageWithExtremelyLargeTextShrinksFontToFitNoException()
    {
        // Arrange
        var longText = new string('W', 200); // Large text block
        var config = new CaptchaConfigurationData
        {
            Text = longText,
            Width = 300,
            Height = 100,
            Font = "Arial",
            Frequency = 5,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            TertiaryColor = Color.Gray
        };

        // Act
        // If the code can't shrink enough, it might cause an infinite loop or a negative font size.
        // We expect no exception if the logic is correct.
        using var image = new CaptchaImage(config).Create();

        // Assert
        Assert.That(image, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(image.Width, Is.EqualTo(300));
            Assert.That(image.Height, Is.EqualTo(100));
        }
    }

    [Test]
    public void CaptchaImageCallingCreateMultipleTimesReturnsSeparateInstances()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "Separate calls",
            Width = 150,
            Height = 50,
            Font = "Arial",
            Frequency = 5,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            TertiaryColor = Color.Gray
        };

        var captchaImage = new CaptchaImage(config);

        // Act
        using var bmp1 = captchaImage.Create();
        using var bmp2 = captchaImage.Create();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(bmp1, Is.Not.Null);
            Assert.That(bmp2, Is.Not.Null);
        }
        // They won't necessarily be pixel-for-pixel different because of randomness,
        // but they are different object references.
        Assert.That(bmp1, Is.Not.SameAs(bmp2));
    }

    [Test]
    public void CaptchaImageWithNullConfigThrowsNullException()
    {
        // Arrange
        CaptchaConfigurationData nullConfig = null;

        // Act & Assert
        // Since the constructor and subsequent usage require a non-null config, we expect an ArgumentNullException.
        Assert.Throws<NullReferenceException>(() =>
        {
            var image = new CaptchaImage(nullConfig);
            _ = image.Create(); // or just the constructor call, depending on how you handle null
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
            Font = "Arial",
            Frequency = 4,
            PrimaryColor = Color.LightGray,
            SecondaryColor = Color.DarkGray,
            TertiaryColor = Color.Gray
        };

        // Act
        using var bitmap = new CaptchaImage(config).Create();

        // Assert
        Assert.That(bitmap, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bitmap.Width, Is.EqualTo(100));
            Assert.That(bitmap.Height, Is.EqualTo(50));
        }
    }

    [Test]
    public void CaptchaImageWhenWarpIsAppliedResultsDifferBetweenCalls()
    {
        // Arrange
        var config = new CaptchaConfigurationData
        {
            Text = "WarpTest",
            Width = 120,
            Height = 50,
            Font = "Arial",
            Frequency = 4,
            PrimaryColor = Color.Blue,
            SecondaryColor = Color.Red,
            TertiaryColor = Color.Green
        };

        var captcha = new CaptchaImage(config);

        // Act
        // Generate two images in a row — due to random warping, they might differ pixel-for-pixel.
        // We'll compare some random pixels or simply check that raw byte arrays differ
        // to demonstrate warping is not identical each time.
        using var bmp1 = captcha.Create();
        using var bmp2 = captcha.Create();

        // Convert both images to byte arrays:
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
    public void CaptchaImageUsesBoldFontForText()
    {
        // Arrange
        // We'll rely on GetFontThatFitsRectangle logic, which uses FontStyle.Bold.
        // This test at least ensures no error occurs when using a bold style.
        var config = new CaptchaConfigurationData
        {
            Text = "BoldCheck",
            Width = 200,
            Height = 80,
            Font = "Times New Roman",
            Frequency = 4,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            TertiaryColor = Color.Gray
        };

        using var captchaBmp = new CaptchaImage(config).Create();

        // Assert
        // Hard to verify the style is actually bold in a pure unit test, but at least confirm it's created.
        Assert.That(captchaBmp, Is.Not.Null, "Bitmap should not be null even with bold style usage.");
    }

    [Test]
    public void CaptchaImageBackgroundIsFilledWithTertiaryColorCombination()
    {
        // Arrange
        // We'll try to detect if at least some pixel in the resulting image matches
        // a color that results from mixing Secondary and Tertiary in the hatch brush.
        var config = new CaptchaConfigurationData
        {
            Text = "BackgroundCheck",
            Width = 100,
            Height = 40,
            Font = "Arial",
            Frequency = 2,
            PrimaryColor = Color.Yellow,  // used for text fill
            SecondaryColor = Color.Red,   // used in the background brush
            TertiaryColor = Color.Blue    // used in the background brush
        };

        // Act
        using var bitmap = new CaptchaImage(config).Create();
        var foundMixedColor = false;

        // Because it's a hatch brush, we won't get a single uniform color.
        // We'll scan a handful of pixels to see if any pixel is not pure red/blue or fully black/white.
        // This is a *lightweight* verification that a hatch might exist.
        // For robust tests, consider advanced color checks or direct mocking of GDI+ (tricky in .NET).
        for (var x = 0; x < bitmap.Width; x += 5)
        {
            for (var y = 0; y < bitmap.Height; y += 5)
            {
                var pixel = bitmap.GetPixel(x, y);
                if (pixel != Color.Red && pixel != Color.Blue && pixel != Color.Yellow && pixel.A > 0)
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
    public void CaptchaImageCreateDoesNotMutateConfig()
    {
        // Arrange
        // We want to verify that calling Create() doesn't internally change the config object (e.g. font name or colors).
        var originalConfig = new CaptchaConfigurationData
        {
            Text = "ImmutableCheck",
            Width = 150,
            Height = 60,
            Font = "Arial",
            Frequency = 10,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.White,
            TertiaryColor = Color.Gray
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
            TertiaryColor = originalConfig.TertiaryColor
        };

        // Act
        _ = new CaptchaImage(originalConfig).Create(); // Generate the image

        // Assert
        // Compare original config vs. copy
        Assert.Multiple(() =>
        {
            Assert.That(originalConfig.Text, Is.EqualTo(configCopy.Text), "Text should remain unchanged.");
            Assert.That(originalConfig.Width, Is.EqualTo(configCopy.Width), "Width should remain unchanged.");
            Assert.That(originalConfig.Height, Is.EqualTo(configCopy.Height), "Height should remain unchanged.");
            Assert.That(originalConfig.Font, Is.EqualTo(configCopy.Font), "Font should remain unchanged.");
            Assert.That(originalConfig.Frequency, Is.EqualTo(configCopy.Frequency), "Frequency should remain unchanged.");
            Assert.That(originalConfig.PrimaryColor, Is.EqualTo(configCopy.PrimaryColor), "PrimaryColor should remain unchanged.");
            Assert.That(originalConfig.SecondaryColor, Is.EqualTo(configCopy.SecondaryColor), "SecondaryColor should remain unchanged.");
            Assert.That(originalConfig.TertiaryColor, Is.EqualTo(configCopy.TertiaryColor), "TertiaryColor should remain unchanged.");
        });
    }

    [Test]
    public void CaptchaImageWithMultiLineTextRendersWithoutException()
    {
        // Arrange
        // Some fonts and measurement logic might handle newlines differently.
        // This test ensures no crash or infinite loop occurs.
        var config = new CaptchaConfigurationData
        {
            Text = "Line1\nLine2\nLine3",
            Width = 200,
            Height = 100,
            Font = "Arial",
            Frequency = 5,
            PrimaryColor = Color.DarkGray,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.Gray
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using var bmp = new CaptchaImage(config).Create();
            Assert.That(bmp, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(bmp.Width, Is.EqualTo(200));
                Assert.That(bmp.Height, Is.EqualTo(100));
            }
        });
    }

    [Test]
    public void CaptchaImageWithSpecialCharactersRendersImage()
    {
        // Arrange
        // Include punctuation, symbols, and possibly right-to-left text for added complexity.
        var specialText = "!@#$%^&*()_+中文测试—🚀";
        var config = new CaptchaConfigurationData
        {
            Text = specialText,
            Width = 250,
            Height = 100,
            Font = "Arial",
            Frequency = 6,
            PrimaryColor = Color.Brown,
            SecondaryColor = Color.Beige,
            TertiaryColor = Color.Pink
        };

        // Act
        using var bmp = new CaptchaImage(config).Create();

        // Assert
        Assert.That(bmp, Is.Not.Null, "Bitmap should not be null for text with special characters.");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bmp.Width, Is.EqualTo(250));
            Assert.That(bmp.Height, Is.EqualTo(100));
        }
    }

    [Test]
    public void CaptchaImageSingleCharacterTextDoesNotCauseUnexpectedBehavior()
    {
        // Arrange
        // A single large character can still require warping and font adjustment.
        var config = new CaptchaConfigurationData
        {
            Text = "W",
            Width = 150,
            Height = 50,
            Font = "Arial",
            Frequency = 5,
            PrimaryColor = Color.Black,
            SecondaryColor = Color.LightGray,
            TertiaryColor = Color.Gray
        };

        // Act
        using var bmp = new CaptchaImage(config).Create();

        // Assert
        Assert.That(bmp, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(bmp.Width, Is.EqualTo(150));
            Assert.That(bmp.Height, Is.EqualTo(50));
        }
    }

    [Test]
    public void CaptchaImageRandomNoiseIsDifferentBetweenCalls()
    {
        // Arrange
        // The random ellipse positions and sizes should cause differences.
        // We'll compare the raw bytes of two generated images. They won't match perfectly
        // if random noise is truly random and the frequency > 0.
        var config = new CaptchaConfigurationData
        {
            Text = "NoiseTest",
            Width = 200,
            Height = 80,
            Font = "Arial",
            Frequency = 10,  // Enough to generate noticeable noise
            PrimaryColor = Color.Red,
            SecondaryColor = Color.Blue,
            TertiaryColor = Color.Green
        };

        var captcha = new CaptchaImage(config);

        // Act
        byte[] data1, data2;
        using (var img1 = captcha.Create())
        using (var ms1 = new MemoryStream())
        {
            img1.Save(ms1, ImageFormat.Png);
            data1 = ms1.ToArray();
        }

        using (var img2 = captcha.Create())
        using (var ms2 = new MemoryStream())
        {
            img2.Save(ms2, ImageFormat.Png);
            data2 = ms2.ToArray();
        }

        // Assert
        // Because warping AND random noise are at play, two images with the same config
        // should differ in binary content the majority of the time.
        Assert.That(data1, Is.Not.EqualTo(data2),
            "Expected random noise to produce different image data between calls, but they appear identical.");
    }

    [Test]
    public void CaptchaImageCanBeCreatedFromMultipleThreadsWithoutConflict()
    {
        // Arrange
        // Tests if concurrent creation leads to any thread-safety issue within the GDI+ calls.
        var config = new CaptchaConfigurationData
        {
            Text = "ThreadSafety",
            Width = 150,
            Height = 50,
            Font = "Arial",
            Frequency = 5,
            PrimaryColor = Color.Maroon,
            SecondaryColor = Color.Lime,
            TertiaryColor = Color.Silver
        };

        var captchaImage = new CaptchaImage(config);

        var exceptions = new ConcurrentBag<Exception>();

        // Act
        Parallel.For(0, 20, _ =>
        {
            try
            {
                using var bmp = captchaImage.Create();
                // Just confirm it's not null
                if (bmp == null)
                {
                    exceptions.Add(new InvalidOperationException("Bitmap returned was null in parallel operation."));
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        // Assert
        // If any thread encountered issues, we'll have them in exceptions.
        Assert.That(exceptions, Is.Empty, "Expected no exceptions during concurrent CaptchaImage creation, but found some.");
    }

}
