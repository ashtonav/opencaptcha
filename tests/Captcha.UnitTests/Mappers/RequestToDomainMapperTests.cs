namespace Captcha.UnitTests.Mappers;

using System;
using System.Collections.Generic;
using System.Linq;
using Captcha.Core;
using Captcha.Core.Mappers;
using Captcha.Core.Models;
using NUnit.Framework;

[TestFixture]
public class RequestToDomainMapperTests
{
    private readonly RequestToDomainMapper _requestToDomainMapper = new();

    [Test]
    public void ToDomainMapsTextCorrectly()
    {
        // Arrange
        var request = new CaptchaRequest { Text = "test text" };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        Assert.That(result.Text, Is.EqualTo("test text"));
    }

    [Test]
    public void ToDomainUsesDefaultWidthAndHeightWhenNotProvided()
    {
        // Arrange
        var request = new CaptchaRequest { Text = "some text" };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(Constants.DefaultCaptchaWidth));
            Assert.That(result.Height, Is.EqualTo(Constants.DefaultCaptchaHeight));
        }
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
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(500));
            Assert.That(result.Height, Is.EqualTo(300));
        }
    }

    [Test]
    public void ToDomainUsesDefaultFrequencyWhenDifficultyNotProvided()
    {
        // Arrange
        var request = new CaptchaRequest { Text = "default freq" };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        Assert.That(result.Frequency, Is.EqualTo(Constants.DefaultFrequency));
    }

    [Test]
    public void ToDomainUsesCorrectFrequencyForDifficulty()
    {
        // Arrange
        var easyRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Easy,
            Text = "easy"
        };

        var mediumRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Medium,
            Text = "medium"
        };

        var challengingRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Challenging,
            Text = "challenging"
        };

        var hardRequest = new CaptchaRequest
        {
            Difficulty = CaptchaDifficulty.Hard,
            Text = "hard"
        };

        // Act
        var easyResult = _requestToDomainMapper.ToDomain(easyRequest);
        var mediumResult = _requestToDomainMapper.ToDomain(mediumRequest);
        var challengingResult = _requestToDomainMapper.ToDomain(challengingRequest);
        var hardResult = _requestToDomainMapper.ToDomain(hardRequest);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(hardResult.Frequency, Is.EqualTo(20F), "Hard frequency");
            Assert.That(easyResult.Frequency, Is.EqualTo(300F), "Easy frequency");
            Assert.That(challengingResult.Frequency, Is.EqualTo(30F), "Challenging frequency");
            Assert.That(mediumResult.Frequency, Is.EqualTo(Constants.DefaultFrequency), "Medium frequency");
        }
    }

    [Test]
    public void ToDomainUsesDefaultFrequencyForUnrecognizedDifficulty()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Difficulty = (CaptchaDifficulty)999,
            Text = "unknown difficulty"
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

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
        var results = requests.Select(_requestToDomainMapper.ToDomain).ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(results[0].Text, Is.EqualTo(requests[0].Text));
            Assert.That(results[0].Width, Is.EqualTo(requests[0].Width));
            Assert.That(results[0].Height, Is.EqualTo(requests[0].Height));
            Assert.That(results[0].Frequency, Is.EqualTo(300F));

            Assert.That(results[1].Text, Is.EqualTo(requests[1].Text));
            Assert.That(results[1].Width, Is.EqualTo(requests[1].Width));
            Assert.That(results[1].Height, Is.EqualTo(requests[1].Height));
            Assert.That(results[1].Frequency, Is.EqualTo(20F));
        }
    }

    [Test]
    public void ToDomainThrowsWhenRequestIsNull()
    {
        // Arrange
        CaptchaRequest request = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _requestToDomainMapper.ToDomain(request));
    }

    [Test]
    public void ToDomainUsesArialUnicodeMsForFontProperty()
    {
        // Arrange
        var request = new CaptchaRequest { Text = Guid.NewGuid().ToString() };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        Assert.That(result.Font, Is.EqualTo(Constants.DefaultCaptchaFontName));
    }

    [Test]
    public void ToDomainUsesDefaultPrimaryColorWhenNotProvided()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = "text"
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.PrimaryColor, Is.EqualTo(Constants.DefaultPrimaryColor));
            Assert.That(result.SecondaryColor, Is.EqualTo(Constants.DefaultSecondaryColor));
            Assert.That(result.ThirdColor, Is.EqualTo(Constants.DefaultTertiaryColor));
        }
    }

    [Test]
    public void ToDomainHandlesNullText()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = null,
            Width = 100,
            Height = 50
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Text, Is.Null);
            Assert.That(result.Width, Is.EqualTo(100));
            Assert.That(result.Height, Is.EqualTo(50));
        }
    }

    [Test]
    public void ToDomainHandlesExcessivelyLargeWidthAndHeight()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = "Large dimensions",
            Width = int.MaxValue,
            Height = int.MaxValue
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(int.MaxValue));
            Assert.That(result.Height, Is.EqualTo(int.MaxValue));
        }
    }

    [Test]
    public void ToDomainPreservesWhitespaceInText()
    {
        // Arrange
        var originalText = "  Leading,  internal   and trailing   whitespace  ";
        var request = new CaptchaRequest
        {
            Text = originalText
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        Assert.That(result.Text, Is.EqualTo(originalText));
    }


    [Test]
    public void ToDomainPreservesUnicodeText()
    {
        // Arrange
        var unicodeString = "Captcha 🚀 Test – 你好";
        var request = new CaptchaRequest
        {
            Text = unicodeString
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        Assert.That(result.Text, Is.EqualTo(unicodeString));
    }

    [Test]
    public void ToDomainAllowsEmptyText()
    {
        // Arrange
        var request = new CaptchaRequest
        {
            Text = string.Empty
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        Assert.That(result.Text, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ToDomainCanHandleExtremelyLongText()
    {
        // Arrange
        var longText = new string('x', 10_000);
        var request = new CaptchaRequest
        {
            Text = longText
        };

        // Act
        var result = _requestToDomainMapper.ToDomain(request);

        // Assert
        Assert.That(result.Text, Has.Length.EqualTo(10_000));
        Assert.That(result.Text, Is.EqualTo(longText));
    }
}
