using Azure.Messaging.ServiceBus;
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
    public async Task Run_Exits_NoMessage()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var testable = new SatelliteRequestProcess(null!, null!, null!, ows.Object);

        // Act
        await testable.Run(null!, null!, null!, ct);

        // Assert
        ows.Verify(v => v.ServiceBusCompleteMessage(It.IsAny<ServiceBusReceiver>(), It.IsAny<OlieServiceBusReceivedMessage<SatelliteRequestQueueModel>>(), ct), Times.Never());
    }

    [Test]
    public async Task Run_Processes_Message()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.SetupSequence(s => s.ServiceBusReceiveJson<SatelliteRequestQueueModel>(null!, ct))
            .ReturnsAsync(new OlieServiceBusReceivedMessage<SatelliteRequestQueueModel>
            {
                ServiceBusReceivedMessage = null!,
                Body = new SatelliteRequestQueueModel()
            })
            .ReturnsAsync((OlieServiceBusReceivedMessage<SatelliteRequestQueueModel>)null!);
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteProductGet(It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(new SatelliteProductEntity { EffectiveDate = "2021-05-18" });
        var business = new Mock<ISatelliteImageBusiness>();
        var config = new Mock<IOlieConfig>();
        var testable = new SatelliteRequestProcess(business.Object, repo.Object, config.Object, ows.Object);

        // Act
        await testable.Run(null!, null!, null!, ct);

        // Assert
        ows.Verify(v => v.ServiceBusCompleteMessage(It.IsAny<ServiceBusReceiver>(), It.IsAny<OlieServiceBusReceivedMessage<SatelliteRequestQueueModel>>(), ct), Times.Once());
    }

    #endregion

    #region Do

    [Test]
    public async Task Do_ThrowsException_ProductNotFound()
    {
        // Arrange
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        var testable = new SatelliteRequestProcess(null!, repo.Object, null!, null!);
        var model = new SatelliteRequestQueueModel();

        // Act, Assert
        Assert.ThrowsAsync<ApplicationException>(() => testable.Do(model, null!, null!, ct));
    }

    [Test]
    public async Task Do_NoException_ProductFound()
    {
        // Arrange
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteProductGet(It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(new SatelliteProductEntity { EffectiveDate = "2021-05-18" });
        var business = new Mock<ISatelliteImageBusiness>();
        var config = new Mock<IOlieConfig>();
        var testable = new SatelliteRequestProcess(business.Object, repo.Object, config.Object, null!);
        var model = new SatelliteRequestQueueModel();

        // Act
        await testable.Do(model, null!, null!, ct);

        // Assert
        business.Verify(v => v.MakePoster(It.IsAny<SatelliteProductEntity>(), It.IsAny<Point>(), It.IsAny<string>(), ct), Times.Once());
    }

    #endregion
}
