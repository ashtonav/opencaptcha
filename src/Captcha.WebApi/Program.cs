using Captcha.WebApi.Dependency;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .AddOpenTelemetryLogs()
    .Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio#add-and-configure-swagger-middleware
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection()
    .AllowAllCors()
    .UseRouting()
    .UseExceptionHandler("/error") // When an error happens, use ErrorController.cs
    .UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();
