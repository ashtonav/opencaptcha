namespace Captcha.UnitTests;

using Core;
using NUnit.Framework;
using SkiaSharp;

[TestFixture]
public class ConstantsTests
{
    [Test]
    public void MaxCaptchaSizeShouldBe1024() => Assert.That(Constants.MaxCaptchaSize, Is.EqualTo(1024));

    [Test]
    public void MinCaptchaSizeShouldBe10() => Assert.That(Constants.MinCaptchaSize, Is.EqualTo(10));

    [Test]
    public void DefaultCaptchaWidthShouldBe400() => Assert.That(Constants.DefaultCaptchaWidth, Is.EqualTo(400));

    [Test]
    public void DefaultCaptchaHeightShouldBe100() => Assert.That(Constants.DefaultCaptchaHeight, Is.EqualTo(100));

    [Test]
    public void DefaultFrequencyShouldBe100() => Assert.That(Constants.DefaultFrequency, Is.EqualTo(100F));

    [Test]
    public void DefaultCaptchaFontNameShouldBeArimo() => Assert.That(Constants.DefaultCaptchaFontName, Is.EqualTo("Arimo"));

    [Test]
    public void FallbackCaptchaFontNameShouldBeUnifont() => Assert.That(Constants.DefaultCaptchaFallbackFontName, Is.EqualTo("Unifont"));


    [Test]
    public void DefaultPrimaryColorShouldBeDarkGray() => Assert.That(Constants.DefaultPrimaryColor, Is.EqualTo(SKColor.Parse("FFD3D3D3")));

    [Test]
    public void DefaultSecondaryColorShouldBeWhite() => Assert.That(Constants.DefaultSecondaryColor, Is.EqualTo(SKColor.Parse("FFFFFFFF")));

    [Test]
    public void CaptchaContentTypeShouldBeImageJpeg() => Assert.That(Constants.CaptchaContentType, Is.EqualTo("image/jpeg"));

    [Test]
    public void WarpCaptchaTextFrequencyShouldBe4() => Assert.That(Constants.WarpCaptchaTextFrequency, Is.EqualTo(4F));

    [Test]
    public void CaptchaNoiseShouldBe50() => Assert.That(Constants.CaptchaNoise, Is.EqualTo(50));
}
