namespace Captcha.UnitTests;

using System.Drawing;
using System.Drawing.Imaging;
using Core;
using NUnit.Framework;

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
    public void DefaultCaptchaFontNameShouldBeArialUnicodeMs() => Assert.That(Constants.DefaultCaptchaFontName, Is.EqualTo("Arial Unicode MS"));

    [Test]
    public void DefaultPrimaryColorShouldBeDarkGray() => Assert.That(Constants.DefaultPrimaryColor, Is.EqualTo(Color.DarkGray));

    [Test]
    public void DefaultSecondaryColorShouldBeLightGray() => Assert.That(Constants.DefaultSecondaryColor, Is.EqualTo(Color.LightGray));

    [Test]
    public void DefaultThirdColorShouldBeWhite() => Assert.That(Constants.DefaultTertiaryColor, Is.EqualTo(Color.White));

    [Test]
    public void CaptchaContentTypeShouldBeImageJpeg() => Assert.That(Constants.CaptchaContentType, Is.EqualTo("image/jpeg"));

    [Test]
    public void CaptchaImageFormatShouldBeJpeg() => Assert.That(Constants.CaptchaImageFormat.Guid, Is.EqualTo(ImageFormat.Jpeg.Guid));

    [Test]
    public void WarpCaptchaTextFrequencyShouldBe4() => Assert.That(Constants.WarpCaptchaTextFrequency, Is.EqualTo(4F));

    [Test]
    public void CaptchaNoiseShouldBe50() => Assert.That(Constants.CaptchaNoise, Is.EqualTo(50));
}
