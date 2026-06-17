namespace Captcha.FunctionalTests.Support;

using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
public class TestBase
{
    protected HttpClient Client { get; set; }
    protected ScenarioContext Context { get; set; }

    protected TestBase(ScenarioContext context)
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();

        Client = webApplicationFactory.CreateClient();
        Context = context;
    }
}
