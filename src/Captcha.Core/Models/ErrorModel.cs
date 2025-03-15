namespace Captcha.Core.Models;

public record ErrorModel(Exception exceptionDetails)
{
    public string Type { get; } = exceptionDetails.GetType().Name;
    public string Message { get; } = exceptionDetails.Message;
    public string StackTrace { get; } = exceptionDetails.ToString();
}
