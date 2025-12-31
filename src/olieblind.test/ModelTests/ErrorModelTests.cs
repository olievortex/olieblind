using olieblind.lib.Models;

namespace olieblind.test.ModelTests;

public class ErrorModelTests
{
    [Test]
    public void From_Model_Exception()
    {
        // Arrange
        var message = Guid.NewGuid().ToString();
        var exception = new ApplicationException(message);

        // Act
        var result = ErrorModel.From(exception);

        // Assert
        Assert.That(result.Message, Is.EqualTo(message));
    }
}
