namespace Captcha.UnitTests.Extensions;
using Captcha.Core.Extensions;
using Core.Models;
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
        var request = new CaptchaRequest() { Text = Guid.NewGuid().ToString() };
        // Act
        var result = request.ToDomain();
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Width, Is.EqualTo(400));
            Assert.That(result.Height, Is.EqualTo(100));
        });

    }

    [Test]
    public void ToDomainUsesDefaultDifficultyWhenNotProvided()
    {
        // Arrange
        var request = new CaptchaRequest() { Text = Guid.NewGuid().ToString() };
        // Act
        var result = request.ToDomain();
        // Assert
        Assert.That(result.Difficulty, Is.EqualTo(CaptchaDifficulty.Medium));
    }

    [Test]
    public void ToDomainUsesArialUnicodeMsForFontProperty()
    {
        // Arrange
        var request = new CaptchaRequest() { Text = Guid.NewGuid().ToString() };
        // Act
        var result = request.ToDomain();
        // Assert
        Assert.That(result.Font, Is.EqualTo("Arial Unicode MS"));
    }

    [Test]
    public void ToDomainUsesProvidedWidthWhenGiven()
    {
        // Arrange
        var request = new CaptchaRequest { Width = 500, Text = Guid.NewGuid().ToString()};
        // Act
        var result = request.ToDomain();
        // Assert
        Assert.That(result.Width, Is.EqualTo(500));
    }

    [Test]
    public void ToDomainUsesProvidedHeightWhenGiven()
    {
        // Arrange
        var request = new CaptchaRequest { Height = 300, Text = Guid.NewGuid().ToString() };
        // Act
        var result = request.ToDomain();
        // Assert
        Assert.That(result.Height, Is.EqualTo(300));
    }

    [Test]
    public void ToDomainUsesProvidedDifficultyWhenGiven()
    {
        // Arrange
        var request = new CaptchaRequest { Difficulty = CaptchaDifficulty.Hard, Text = Guid.NewGuid().ToString() };
        // Act
        var result = request.ToDomain();
        // Assert
        Assert.That(result.Difficulty, Is.EqualTo(CaptchaDifficulty.Hard));
    }

    [Test]
    public void ToDomainMapsMultipleRequestsCorrectly()
    {
        // Arrange
        var requests = new List<CaptchaRequest>
        {
            new() {
                Text = "test1",
                Width = 500,
                Height = 200,
                Difficulty = CaptchaDifficulty.Easy
            },
            new() {
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
            Assert.That(results[0].Difficulty, Is.EqualTo(requests[0].Difficulty));

            Assert.That(results[1].Text, Is.EqualTo(requests[1].Text));
            Assert.That(results[1].Width, Is.EqualTo(requests[1].Width));
            Assert.That(results[1].Height, Is.EqualTo(requests[1].Height));
            Assert.That(results[1].Difficulty, Is.EqualTo(requests[1].Difficulty));
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
}
