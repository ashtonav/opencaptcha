namespace Captcha.FunctionalTests.StepDefinitions;

using System.Drawing;
using System.Globalization;
using Core.Models;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using Support;

[Binding]
public class CaptchaSteps(ScenarioContext context) : TestBase(context)
{
    private CaptchaRequest _request;
    private RestResponse _response;

    [Given(@"I have a captcha request with following parameters:")]
    public void GivenIHaveACaptchaRequestWithFollowingParameters(Table table)
    {
        var row = table.Rows[0];

        _request = new CaptchaRequest
        {
            Text = row["Text"],
            Width = string.IsNullOrEmpty(row["Width"]) ? null : int.Parse(row["Width"], CultureInfo.InvariantCulture),
            Height = string.IsNullOrEmpty(row["Height"]) ? null : int.Parse(row["Height"], CultureInfo.InvariantCulture),
            Difficulty = string.IsNullOrEmpty(row["Difficulty"]) ? null
                : Enum.Parse<CaptchaDifficulty>(row["Difficulty"], true)
        };
    }

    [When(@"I send the request to the Create endpoint of the CaptchaController")]
    public async Task WhenISendTheRequestToTheCreateEndpointOfTheCaptchaController()
    {
        var request = new RestRequest("captcha") // calls localhost/captcha
        { RequestFormat = DataFormat.Json, Method = Method.Post };
        request.AddJsonBody(_request);
        _response = await Client.ExecuteAsync(request);
    }

    [Then(@"I expect a captcha image to be returned with the following attributes:")]
    public void ThenIExpectACaptchaImageToBeReturnedWithTheFollowingAttributes(Table table)
    {
        var row = table.Rows[0];
        using var ms = new MemoryStream(_response.RawBytes);
        var img = Image.FromStream(ms);

        var expectedWidth = int.Parse(row["Width"], CultureInfo.InvariantCulture);
        var expectedHeight = int.Parse(row["Height"], CultureInfo.InvariantCulture);

        Assert.That(img.Width, Is.EqualTo(expectedWidth));
        Assert.That(img.Height, Is.EqualTo(expectedHeight));
    }

    [Then(@"I expect a captcha image to be returned without any black borders")]
    public void ThenIExpectACaptchaImageToBeReturnedWithoutAnyBlackBorders()
    {
        using var ms = new MemoryStream(_response.RawBytes);
        var img = Image.FromStream(ms);
        var bmp = new Bitmap(img);

        for (var i = 0; i < bmp.Width; i++)
        {
            for (var j = 0; j < bmp.Height; j++)
            {
                var pixel = bmp.GetPixel(i, j);

                // If either R or G or B is less than 100, then it's a dark color
                if (pixel.R < 100 || pixel.G < 100 || pixel.B < 100)
                {
                    throw new AssertionException($"Black/Dark color found in the image. Hex: {pixel.Name}");
                }
            }
        }
    }
}
