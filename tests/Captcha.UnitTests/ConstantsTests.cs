namespace Captcha.UnitTests;

using System.Drawing;
using Core;
using NUnit.Framework;

[TestFixture]
public class ConstantsTests
{
    [Test]
    public void MaxCaptchaSizeShouldBe1024() => Assert.That(Constants.MaxCaptchaSize, Is.EqualTo(1024));

    [Test]
    public void MinCaptchaSizeShouldBe10() => Assert.That(Constants.MaxCaptchaSize, Is.EqualTo(10));

    [Test]
    public void DefaultCaptchaWidthShouldBe400() => Assert.That(Constants.DefaultCaptchaWidth, Is.EqualTo(400));

    [Test]
    public void DefaultCaptchaHeightShouldBe100() => Assert.That(Constants.DefaultCaptchaHeight, Is.EqualTo(100));

    [Test]
    public void DefaultFrequencyShouldBe100() => Assert.That(Constants.DefaultFrequency, Is.EqualTo(100F));

    [Test]
    public void DefaultCaptchaFontNameShouldBeArialUnicodeMs() => Assert.That(Constants.DefaultCaptchaFontName, Is.EqualTo("Arial Unicode MS"));

    [Test]
    public void DefaultPrimaryColorShouldBeDarkGray() => Assert.That(Constants.DefaultPrimaryColor, Is.EqualTo(Color.DarkGray));

    [Test]
    public void DefaultSecondaryColorShouldBeLightGray() => Assert.That(Constants.DefaultSecondaryColor, Is.EqualTo(Color.LightGray));

    [Test]
    public void DefaultThirdColorShouldBeWhite() => Assert.That(Constants.DefaultThirdColor, Is.EqualTo(Color.White));
}
