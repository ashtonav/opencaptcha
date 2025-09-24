namespace Captcha.FunctionalTests.StepDefinitions;

using System.Globalization;
using Core.Models;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using SkiaSharp;
using Support;

[Binding]
public class CaptchaSteps(ScenarioContext context) : TestBase(context)
{
    private GetCreateCaptchaRequest? _getRequest;
    private PostCreateCaptchaRequest? _postRequest;

    private RestResponse? _response;

    [Given(@"I have a captcha request with following parameters:")]
    public void GivenIHaveACaptchaRequestWithFollowingParameters(Table table)
    {
        var row = table.Rows[0];

        _postRequest = new PostCreateCaptchaRequest
        {
            Text = row[TestConstants.Text],
            Width = string.IsNullOrEmpty(row[TestConstants.Width])
                ? null
                : int.Parse(row[TestConstants.Width], CultureInfo.InvariantCulture),
            Height = string.IsNullOrEmpty(row[TestConstants.Height])
                ? null
                : int.Parse(row[TestConstants.Height], CultureInfo.InvariantCulture),
            Difficulty = string.IsNullOrEmpty(row[TestConstants.Difficulty])
                ? null
                : Enum.Parse<CaptchaDifficulty>(row[TestConstants.Difficulty], true),
            Theme = new ThemeConfiguration()
            {
                PrimaryColor = !row.ContainsKey(TestConstants.PrimaryColor)
                ? null
                : row[TestConstants.PrimaryColor],
                SecondaryColor = !row.ContainsKey(TestConstants.SecondaryColor)
                ? null
                : row[TestConstants.SecondaryColor]
            }
        };
    }

    [When(@"I send the request to the Create endpoint of the CaptchaController")]
    public async Task WhenISendTheRequestToTheCreateEndpointOfTheCaptchaController()
    {
        var request = new RestRequest(TestConstants.CreateCaptchaEndpoint)
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Post
        }.AddJsonBody(_postRequest);

        _response = await Client.ExecuteAsync(request);
    }

    [Then(@"I expect a captcha image to be returned with the following attributes:")]
    public void ThenIExpectACaptchaImageToBeReturnedWithTheFollowingAttributes(Table table)
    {
        var row = table.Rows[0];
        using var ms = new MemoryStream(_response.RawBytes);
        var img = SKImage.FromEncodedData(ms);

        var expectedWidth = int.Parse(row[TestConstants.Width], CultureInfo.InvariantCulture);
        var expectedHeight = int.Parse(row[TestConstants.Height], CultureInfo.InvariantCulture);

        Assert.That(img.Width, Is.EqualTo(expectedWidth));
        Assert.That(img.Height, Is.EqualTo(expectedHeight));
    }

    [Then(@"I expect a captcha image to be returned without any black borders")]
    public void ThenIExpectACaptchaImageToBeReturnedWithoutAnyBlackBorders()
    {
        using var ms = new MemoryStream(_response!.RawBytes!);
        var img = SKImage.FromEncodedData(ms);
        var bmp = SKBitmap.FromImage(img);

        for (var i = 0; i < bmp.Width; i++)
        {
            for (var j = 0; j < bmp.Height; j++)
            {
                var pixel = bmp.GetPixel(i, j);

                // If either R or G or B is less than 100, then it's a dark color
                if (pixel.Red < 100 || pixel.Green < 100 || pixel.Blue < 100)
                {
                    throw new AssertionException($"Black/Dark color found in the image. Hex: {pixel}");
                }
            }
        }
    }

    [Then("I expect a captcha image to contain at least {string} pixels of color {string} and at least {string} pixels of color {string}")]
    public void ThenIExpectACaptchaImageToContainPixelsOfColorAndPixelsOfColor
        (string firstColorAmountOfPixels, string firstColorHex, string secondColorAmountOfPixels, string secondColorHex)
    {
        var firstColor = SKColor.Parse(firstColorHex);
        var firstColorExpectedAmount = int.Parse(firstColorAmountOfPixels, CultureInfo.InvariantCulture);

        var secondColor = SKColor.Parse(secondColorHex);
        var secondColorExpectedAmount = int.Parse(secondColorAmountOfPixels, CultureInfo.InvariantCulture);

        using var ms = new MemoryStream(_response!.RawBytes!);
        var img = SKImage.FromEncodedData(ms);
        var bmp = SKBitmap.FromImage(img);

        var firstColorActualAmount = 0;
        var secondColorActualAmount = 0;

        for (var i = 0; i < bmp.Width; i++)
        {
            for (var j = 0; j < bmp.Height; j++)
            {
                var pixel = bmp.GetPixel(i, j);

                if (pixel == firstColor)
                {
                    firstColorActualAmount++;
                }
                else if (pixel == secondColor)
                {
                    secondColorActualAmount++;
                }
            }
        }

        Assert.That(firstColorActualAmount, Is.AtLeast(firstColorExpectedAmount));
        Assert.That(secondColorActualAmount, Is.AtLeast(secondColorExpectedAmount));
    }

    [Given("I have a captcha request using get with following parameters:")]
    public void GivenIHaveACaptchaRequestUsingGetWithFollowingParameters(Table table)
    {
        var row = table.Rows[0];

        _getRequest = new GetCreateCaptchaRequest
        {
            Text = row[TestConstants.Text],
            Width = string.IsNullOrEmpty(row[TestConstants.Width])
                ? null
                : int.Parse(row[TestConstants.Width], CultureInfo.InvariantCulture),
            Height = string.IsNullOrEmpty(row[TestConstants.Height])
                ? null
                : int.Parse(row[TestConstants.Height], CultureInfo.InvariantCulture),
            Difficulty = string.IsNullOrEmpty(row[TestConstants.Difficulty])
                ? null
                : Enum.Parse<CaptchaDifficulty>(row[TestConstants.Difficulty], true),
            Theme = new ThemeConfiguration()
            {
                PrimaryColor = !row.ContainsKey(TestConstants.PrimaryColor)
                    ? null
                    : row[TestConstants.PrimaryColor],
                SecondaryColor = !row.ContainsKey(TestConstants.SecondaryColor)
                    ? null
                    : row[TestConstants.SecondaryColor]
            }
        };
    }

    [When("I send the get request to the Create endpoint of the CaptchaController")]
    public async Task WhenISendTheGetRequestToTheCreateEndpointOfTheCaptchaController()
    {
        var request = new RestRequest(TestConstants.CreateCaptchaEndpoint)
            .AddObject<GetCreateCaptchaRequest>(_getRequest);

        _response = await Client.ExecuteAsync(request);
    }
}
