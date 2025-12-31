using olieblind.lib.ForecastModels;

namespace olieblind.test.ForecastModelsTests;

public class ProductTests
{
    [Test]
    public void Product_Record_Setters()
    {
        // Arrange
        var product = new Product(1, "file.nc", "Test Product");

        // Act
        product = product with { Id = 2, File = "newfile.nc", Title = "New Product" };

        // Assert
        Assert.That(product.Id, Is.EqualTo(2));
        Assert.That(product.File, Is.EqualTo("newfile.nc"));
        Assert.That(product.Title, Is.EqualTo("New Product"));
    }
}
