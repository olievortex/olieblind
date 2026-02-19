using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Models;
using olieblind.lib.Processes;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;
using System.Drawing;

namespace olieblind.test.ProcessesTests;

public class SatelliteRequestProcessTests
{
    #region Run

    [Test]
    public async Task Run_ThrowsException_ProductNotFound()
    {
        // Arrange
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteRequestProcess(null!, repo.Object, null!);
        var model = new SatelliteRequestQueueModel();

        // Act, Assert
        Assert.ThrowsAsync<ApplicationException>(() => testable.Run(model, null!, null!, ct));
    }

    [Test]
    public async Task Run_NoException_ProductFound()
    {
        // Arrange
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteProductGet(It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(new SatelliteProductEntity { EffectiveDate = "2021-05-18" });
        var business = new Mock<ISatelliteImageBusiness>();
        var config = new Mock<IOlieConfig>();
        var testable = new SatelliteRequestProcess(business.Object, repo.Object, config.Object);
        var model = new SatelliteRequestQueueModel();

        // Act
        await testable.Run(model, null!, null!, ct);

        // Assert
        business.Verify(v => v.MakePoster(It.IsAny<SatelliteProductEntity>(), It.IsAny<Point>(), It.IsAny<string>(), ct), Times.Once());
    }

    #endregion
}
