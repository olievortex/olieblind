namespace olieblind.test.StormEventsTests.ModelsTests;

public class AnnualOverviewModelTests
{
    [Test]
    public void AnnualOverviewItem_Initializes_DefaultConstructor()
    {
        // Arrange & Act
        var item = new olieblind.lib.StormEvents.Models.AnnualOverviewModel.AnnualOverviewItem();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(item.Id, Is.EqualTo(string.Empty));
            Assert.That(item.SourceFk, Is.EqualTo(string.Empty));
            Assert.That(item.HailCount, Is.Zero);
            Assert.That(item.TornadoCount, Is.Zero);
            Assert.That(item.WindCount, Is.Zero);
        }
    }
}
