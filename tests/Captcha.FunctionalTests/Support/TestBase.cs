namespace Captcha.FunctionalTests.Support;

using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using RestSharp;
public class TestBase
{
    protected RestClient Client { get; set; }
    protected ScenarioContext Context { get; set; }

    protected TestBase(ScenarioContext context)
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();

        Client = new RestClient(webApplicationFactory.CreateClient());
        Context = context;
    }
}
