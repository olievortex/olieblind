using Moq;
using olieblind.data.Enums;
using olieblind.lib.Satellite;
using olieblind.lib.Satellite.Interfaces;
using olieblind.test.SatelliteTests.SourcesTests;

namespace olieblind.test.SatelliteTests;

public class SatelliteImageProcessTests
{
    #region DownloadInventory

    [Test]
    public async Task DownloadInventory_ShortCircuit_NoKeys()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-21";
        const int satellite = 16;
        const int channel = 99;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var business = new Mock<ISatelliteImageBusiness>();
        var source = new SatelliteTestSource { Ows = null! };
        var testable = new SatelliteImageProcess(business.Object);

        // Act
        await testable.DownloadInventory(effectiveDate, satellite, channel, dayPart, source, ct);

        // Assert
        business.Verify(v => v.AddInventoryToDatabase(It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int>(), It.IsAny<DayPartsEnum>(), ct), Times.Never);
    }

    [Test]
    public async Task DownloadInventory_Finishes_HasKeys()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-21";
        const int satellite = 16;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var business = new Mock<ISatelliteImageBusiness>();
        var source = new SatelliteTestSource { Ows = null! };
        var testable = new SatelliteImageProcess(business.Object);

        // Act
        await testable.DownloadInventory(effectiveDate, satellite, channel, dayPart, source, ct);

        // Assert
        business.Verify(v => v.AddInventoryToDatabase("2021-07-21", "", channel, dayPart, ct), Times.Once);
    }

    #endregion
}
