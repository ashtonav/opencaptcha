namespace Captcha.UnitTests.Extensions;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Captcha.Core.Extensions;
using Captcha.Core.Models;
using Core;
using NUnit.Framework;

[TestFixture]
public class MappingExtensionsTests
{
    [Test]
    public void ToDomainMapsTextCorrectly()
    {
        // Arrange
        var request = new CaptchaRequest { Text = "test text" };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.Text, Is.EqualTo("test text"));
    }

    [Test]
    public void ToDomainUsesDefaultWidthAndHeightWhenNotProvided()
    {
        // Arrange
        var request = new CaptchaRequest { Text = "some text" };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Width, Is.EqualTo(Constants.DefaultCaptchaWidth));
            Assert.That(result.Height, Is.EqualTo(Constants.DefaultCaptchaHeight));
        });
    }

    [Test]
    public void ToDomainUsesProvidedWidthAndHeight()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Width = 500,
            Height = 300,
            Text = "another text"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Width, Is.EqualTo(500));
            Assert.That(result.Height, Is.EqualTo(300));
        });
    }

    [Test]
    public void ToDomainUsesDefaultFrequencyWhenDifficultyNotProvided()
    {
        // Arrange
        var request = new CaptchaRequest { Text = "default freq" };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.Frequency, Is.EqualTo(Constants.DefaultFrequency));
    }

    [Test]
    public void ToDomainUsesCorrectFrequencyForDifficulty()
    {
        // Arrange
        var hardRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Hard,
            Text = "hard"
        };

        var easyRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Easy,
            Text = "easy"
        };

        var challengingRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Challenging,
            Text = "challenging"
        };

        var mediumRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Medium,
            Text = "medium"
        };

        // Act
        var hardResult = hardRequest.ToDomain();
        var easyResult = easyRequest.ToDomain();
        var challengingResult = challengingRequest.ToDomain();
        var mediumResult = mediumRequest.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(hardResult.Frequency, Is.EqualTo(20F), "Hard frequency");
            Assert.That(easyResult.Frequency, Is.EqualTo(300F), "Easy frequency");
            Assert.That(challengingResult.Frequency, Is.EqualTo(30F), "Challenging frequency");
            Assert.That(mediumResult.Frequency, Is.EqualTo(Constants.DefaultFrequency), "Medium frequency");
        });
    }

    [Test]
    public void ToDomainUsesDefaultFrequencyForUnrecognizedDifficulty()
    {
        // Arrange
        // Casting an out-of-range value to CaptchaDifficulty for testing:
        var request = new CaptchaRequest
        {
            Difficulty = (CaptchaDifficulty)999,
            Text = "unknown difficulty"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.Frequency, Is.EqualTo(Constants.DefaultFrequency));
    }

    [Test]
    public void ToDomainMapsMultipleRequestsCorrectly()
    {
        // Arrange
        var requests = new List<CaptchaRequest>
        {
            new()
            {
                Text = "test1",
                Width = 500,
                Height = 200,
                Difficulty = CaptchaDifficulty.Easy
            },
            new()
            {
                Text = "test2",
                Width = 600,
                Height = 300,
                Difficulty = CaptchaDifficulty.Hard
            },
        };

        // Act
        var results = requests.Select(r => r.ToDomain()).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Text, Is.EqualTo(requests[0].Text));
            Assert.That(results[0].Width, Is.EqualTo(requests[0].Width));
            Assert.That(results[0].Height, Is.EqualTo(requests[0].Height));
            Assert.That(results[0].Frequency, Is.EqualTo(300F));

            Assert.That(results[1].Text, Is.EqualTo(requests[1].Text));
            Assert.That(results[1].Width, Is.EqualTo(requests[1].Width));
            Assert.That(results[1].Height, Is.EqualTo(requests[1].Height));
            Assert.That(results[1].Frequency, Is.EqualTo(20F));
        });
    }

    [Test]
    public void ToDomainThrowsWhenRequestIsNull()
    {
        // Arrange
        CaptchaRequest request = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => request.ToDomain());
    }

    [Test]
    public void ToDomainUsesArialUnicodeMsForFontProperty()
    {
        // Arrange
        var request = new CaptchaRequest { Text = Guid.NewGuid().ToString() };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.Font, Is.EqualTo(Constants.DefaultCaptchaFontName));
    }

    [Test]
    public void ToDomainUsesDefaultPrimaryColorWhenNotProvided()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = "text",
            Colors = new ColorSettings
            {
                Primary = null,
                Secondary = "#FF0000",
                Tertiary = "#0000FF"
            }
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PrimaryColor, Is.EqualTo(Constants.DefaultPrimaryColor));
            Assert.That(result.SecondaryColor, Is.EqualTo(Color.Red));
            Assert.That(result.TertiaryColor, Is.EqualTo(Color.Blue));
        });
    }

    [Test]
    public void ToDomainUsesSpecifiedColorsWhenProvided()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = "text",
            Colors = new ColorSettings
            {
                Primary = "#123456",
                Secondary = "#654321",
                Tertiary = "Green"
            }
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            // #123456 -> R=18, G=52, B=86
            Assert.That(result.PrimaryColor, Is.EqualTo(Color.FromArgb(0x12, 0x34, 0x56)));
            // #654321 -> R=101, G=67, B=33
            Assert.That(result.SecondaryColor, Is.EqualTo(Color.FromArgb(0x65, 0x43, 0x21)));
            // Named color "Green"
            Assert.That(result.TertiaryColor, Is.EqualTo(Color.Green));
        });
    }

    [Test]
    public void ToDomainUsesDefaultColorsWhenEmptyOrWhitespace()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = "text",
            Colors = new ColorSettings
            {
                Primary = " ",
                Secondary = "",
                Tertiary = null
            }
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PrimaryColor, Is.EqualTo(Constants.DefaultPrimaryColor));
            Assert.That(result.SecondaryColor, Is.EqualTo(Constants.DefaultSecondaryColor));
            Assert.That(result.TertiaryColor, Is.EqualTo(Constants.DefaultThirdColor));
        });
    }

    [Test]
    public void ToDomainUsesDefaultColorsWhenColorsIsNull()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = "no colors",
            Colors = null
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PrimaryColor, Is.EqualTo(Constants.DefaultPrimaryColor));
            Assert.That(result.SecondaryColor, Is.EqualTo(Constants.DefaultSecondaryColor));
            Assert.That(result.TertiaryColor, Is.EqualTo(Constants.DefaultThirdColor));
        });
    }

    [Test]
    public void ToDomainParsesAlphaHexColor()
    {
        // Arrange
        // Example: #80FF0000 = 50% opaque red
        var request = new CaptchaRequest
        {
            Text = "alpha color",
            Colors = new ColorSettings
            {
                Primary = "#80FF0000"
            }
        };

        // Act
        var result = request.ToDomain();

        // Assert
        var expectedColor = Color.FromArgb(0x80, 0xFF, 0x00, 0x00);
        Assert.That(result.PrimaryColor, Is.EqualTo(expectedColor));
    }

    [Test]
    public void ToDomainAllowsNegativeOrZeroDimensionsIfThatIsDesired()
    {
        // Arrange
        // This test depends on how you want to handle zero or negative dimensions.
        // If you want to clamp to a minimum, you could test for that. Currently, the
        // extension method doesn't do any range checking—it just passes the values through.
        // Adjust the expected behavior as needed.

        var request = new CaptchaRequest
        {
            Width = 0,
            Height = -100,
            Text = "Dimension edge case"
        };

        // Act
        var result = request.ToDomain();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            // If you allow zero/negative:
            Assert.That(result.Width, Is.EqualTo(0));
            Assert.That(result.Height, Is.EqualTo(-100));
        }

        // If you'd rather clamp or throw, adjust the code and test accordingly:
        // Assert.That(result.Width, Is.EqualTo(Constants.DefaultCaptchaWidth));
        // Assert.That(result.Height, Is.EqualTo(Constants.DefaultCaptchaHeight));
    }


    [Test]
    public void ToDomainHandlesNullText()
    {
        // Arrange
        // We deliberately leave Text as null to see how the method handles it.
        var request = new CaptchaRequest
        {
            Text = null,
            Width = 100,
            Height = 50
        };

        // Act
        var result = request.ToDomain();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            // Expecting that null simply maps through to the result.
            Assert.That(result.Text, Is.Null);
            Assert.That(result.Width, Is.EqualTo(100));
            Assert.That(result.Height, Is.EqualTo(50));
        }
    }

    [Test]
    public void ToDomainHandlesCaseInsensitiveNamedColor()
    {
        // Arrange
        // Here "rEd" is a named color, but with mixed casing.
        var request = new CaptchaRequest
        {
            Text = "Case-insensitive color",
            Colors = new ColorSettings
            {
                Primary = "rEd"  // equivalent to Color.Red
            }
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.PrimaryColor, Is.EqualTo(Color.Red));
    }


    [Test]
    public void ToDomainReturnsDistinctInstancesForEachCall()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = "Some text",
            Width = 200
        };

        // Act
        var domainObject1 = request.ToDomain();
        var domainObject2 = request.ToDomain();

        // Assert
        // Even though the data is the same, we want to ensure a new object is returned each time.
        Assert.That(domainObject1, Is.Not.SameAs(domainObject2));
    }

    [Test]
    public void ToDomainNoMutationOnOriginalRequest()
    {
        // Arrange
        // We verify that converting the request to domain doesn't alter the original request object.
        var request = new CaptchaRequest
        {
            Text = "Original",
            Width = 123,
            Height = 456,
            Difficulty = CaptchaDifficulty.Hard,
            Colors = new ColorSettings
            {
                Primary = "#FF0000",
                Secondary = "#00FF00",
                Tertiary = "#0000FF"
            }
        };

        // Make a clone in memory so we can compare post-call
        var originalText = request.Text;
        var originalWidth = request.Width;
        var originalHeight = request.Height;
        var originalDifficulty = request.Difficulty;
        var originalPrimary = request.Colors.Primary;
        var originalSecondary = request.Colors.Secondary;
        var originalTertiary = request.Colors.Tertiary;

        // Act
        _ = request.ToDomain();

        // Assert
        // Confirm the request object was not changed.
        Assert.Multiple(() =>
        {
            Assert.That(request.Text, Is.EqualTo(originalText));
            Assert.That(request.Width, Is.EqualTo(originalWidth));
            Assert.That(request.Height, Is.EqualTo(originalHeight));
            Assert.That(request.Difficulty, Is.EqualTo(originalDifficulty));
            Assert.That(request.Colors.Primary, Is.EqualTo(originalPrimary));
            Assert.That(request.Colors.Secondary, Is.EqualTo(originalSecondary));
            Assert.That(request.Colors.Tertiary, Is.EqualTo(originalTertiary));
        });
    }

    [Test]
    public void ToDomainHandlesExcessivelyLargeWidthAndHeight()
    {
        // Arrange
        // Test very large dimension values to ensure the code doesn't break or overflow.
        var request = new CaptchaRequest
        {
            Text = "Large dimensions",
            Width = int.MaxValue,
            Height = int.MaxValue
        };

        // Act
        var result = request.ToDomain();

        // Assert
        // By default, the method just passes them through. If you eventually want
        // to clamp or validate them, update both the code and this test accordingly.
        Assert.Multiple(() =>
        {
            Assert.That(result.Width, Is.EqualTo(int.MaxValue));
            Assert.That(result.Height, Is.EqualTo(int.MaxValue));
        });
    }

    [Test]
    public void ToDomainPreservesWhitespaceInText()
    {
        // Arrange
        // Some scenarios may need to preserve spacing, newlines, etc.
        // This test ensures the text is transferred verbatim.
        var originalText = "  Leading,  internal   and trailing   whitespace  ";
        var request = new CaptchaRequest
        {
            Text = originalText
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.Text, Is.EqualTo(originalText));
    }


    [Test]
    public void ToDomainPreservesUnicodeText()
    {
        // Arrange
        // Example of Unicode text with emojis
        var unicodeString = "Captcha 🚀 Test – 你好";
        var request = new CaptchaRequest
        {
            Text = unicodeString
        };

        // Act
        var result = request.ToDomain();

        // Assert
        // Verifying the text remains intact
        Assert.That(result.Text, Is.EqualTo(unicodeString));
    }

    [Test]
    public void ToDomainAllowsEmptyText()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = string.Empty  // specifically an empty string
        };

        // Act
        var result = request.ToDomain();

        // Assert
        // Confirming that an empty string maps through without errors
        Assert.That(result.Text, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ToDomainCanHandleExtremelyLongText()
    {
        // Arrange
        // Construct a very large string (e.g., 10,000 characters)
        var longText = new string('x', 10_000);
        var request = new CaptchaRequest
        {
            Text = longText
        };

        // Act
        var result = request.ToDomain();

        // Assert
        // Ensuring the text is preserved as-is
        Assert.That(result.Text, Has.Length.EqualTo(10_000));
        Assert.That(result.Text, Is.EqualTo(longText));
    }

    [Test]
    public void ToDomainCanBeCalledFromMultipleThreads()
    {
        // Arrange
        // We'll run multiple parallel calls to see if any race conditions occur.
        // Note: ColorConverter is not guaranteed thread-safe, but often works fine.
        // If you see concurrency exceptions, you might need a lock or create a new instance per call.
        var request = new CaptchaRequest
        {
            Text = "Thread test",
            Colors = new ColorSettings
            {
                Primary = "#FF00FF"
            }
        };

        var exceptions = new List<Exception>();

        // Act
        Parallel.For(0, 50, _ =>
        {
            try
            {
                // If anything goes wrong, store exception
                request.ToDomain();
            }
            catch (Exception ex)
            {
                lock (exceptions)
                {
                    exceptions.Add(ex);
                }
            }
        });

        // Assert
        // We expect zero exceptions if the method is thread-safe enough for concurrent use.
        Assert.That(exceptions, Is.Empty, "Concurrent calls threw exceptions");
    }

    [Test]
    public void ToDomainThrowsForUnsupportedColorFormatRgbFunction()
    {
        // Arrange
        // "ColorConverter" in .NET does not typically handle CSS-style "rgb(255,0,0)" strings by default.
        // Expecting a FormatException here, unless your code specifically supports it.
        var request = new CaptchaRequest
        {
            Text = "RGB color test",
            Colors = new ColorSettings
            {
                Primary = "rgb(255,0,0)"
            }
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => request.ToDomain());
    }

}
