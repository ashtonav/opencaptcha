namespace Captcha.UnitTests.Models;

using System;
using Captcha.Core.Models;
using NUnit.Framework;

[TestFixture]
public class ErrorModelTests
{
    [TestCase(typeof(InvalidOperationException), "Unsupported operation")]
    [TestCase(typeof(ArgumentNullException), "Argument cannot be null")]
    [TestCase(typeof(FormatException), "Invalid format")]
    public void ErrorModelConstructedWithExceptionExceptionPropertiesMatchModelProperties(Type exceptionType, string message)
    {
        // Arrange
        var comparedException = (Exception)Activator.CreateInstance(exceptionType, message)!;

        // Act
        var errorModel = new ErrorModel(comparedException);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(errorModel.Type, Is.EqualTo(comparedException.GetType().Name));
            Assert.That(errorModel.Message, Is.EqualTo(comparedException.Message));
            Assert.That(errorModel.StackTrace, Is.EqualTo(comparedException.ToString()));
        }
    }
}
