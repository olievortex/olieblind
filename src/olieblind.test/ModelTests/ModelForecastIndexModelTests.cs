namespace olieblind.test.ModelTests;

public class ModelForecastIndexModelTests
{
    [Test]
    public void GetItemByParameterId_ReturnsValue_ValidParameterId()
    {
        // Arrange
        var model = new olieblind.lib.Models.ModelForecastIndexModel
        {
            Items =
            [
                new() { ParameterId = 1, Title = "One", Url = "http://one.com" },
                new() { ParameterId = 2, Title = "Two", Url = "http://two.com" },
                new() { ParameterId = 3, Title = "Three", Url = "http://three.com" },
            ]
        };

        // Act
        var result = model.GetItemByParameterId(2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ParameterId, Is.EqualTo(2));
            Assert.That(result.Title, Is.EqualTo("Two"));
            Assert.That(result.Url, Is.EqualTo("http://two.com"));
        }
    }
}
