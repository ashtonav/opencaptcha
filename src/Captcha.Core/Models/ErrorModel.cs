namespace Captcha.Core.Models;

public record ErrorModel(Exception ExceptionDetails)
{
    public string Type { get; } = ExceptionDetails.GetType().Name;
    public string Message { get; } = ExceptionDetails.Message;
    public string StackTrace { get; } = ExceptionDetails.ToString();
}
