using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Maintenance;
using olieblind.lib.Processes;
using olieblind.lib.Services;

namespace olieblind.test.ProcessesTests;

public class DeleteOldContentProcessTests
{
    [Test]
    public async Task Run_DeletesFiles_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var timestamp = DateTime.UtcNow.AddDays(-61);
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductVideoGetList(ct))
            .ReturnsAsync([
                new ProductVideoEntity { Category = "Drought Monitor", Timestamp = timestamp },
                new ProductVideoEntity { Category = "Dillon's Drought Monitor", Timestamp = timestamp }
                ]);
        repo.Setup(s => s.ProductMapList(ct))
            .ReturnsAsync([
                new ProductMapEntity { Id = 12, Timestamp = timestamp }
                ]);
        repo.Setup(s => s.ProductMapItemList(12, ct))
            .ReturnsAsync([
                new ProductMapItemEntity { LocalPath = @"C:\temp\file1.png" },
                new ProductMapItemEntity { LocalPath = @"C:\temp\file2.png" }
                ]);
        var ows = new Mock<IOlieWebService>();
        var mysql = new Mock<IMySqlMaintenance>();
        mysql.Setup(s => s.GetBackups())
            .Returns([
                new BackupFile { BackupFilePath = $"{DateTime.UtcNow.AddDays(-8):yyyyMMdd}_010000_olieblind_dev.sql" },
                new BackupFile { BackupFilePath = $"{DateTime.UtcNow.AddDays(-6):yyyyMMdd}_010000_olieblind_dev.sql"  },
                new BackupFile { BackupFilePath = $"{DateTime.UtcNow.AddDays(-10):yyyyMMdd}_010000_olieblind_dev.sql"  }
                ]);
        var process = new DeleteOldContentProcess(repo.Object, ows.Object, mysql.Object);

        // Act
        await process.Run(null!, ct);

        // Assert
        ows.Verify(v => v.FileDelete(It.IsAny<string>()), Times.Exactly(11));
    }

    [Test]
    public void SafeDeleteFile_NoExceptionThrown_NullOrEmptyPath()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.FileDelete(It.IsAny<string>()))
            .Throws(new ApplicationException("File not found"));
        var process = new DeleteOldContentProcess(repo.Object, ows.Object, null!);

        // Act & Assert
        Assert.DoesNotThrow(() => process.SafeDeleteFile(null!));
    }
    
    #region SafeDelete Folder
    
    [Test]
    public void SafeDeleteFolder_NoExceptionThrown_ServiceThrows()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.DirectoryDelete(It.IsAny<string>()))
            .Throws(new ApplicationException("Directory not found"));
        var process = new DeleteOldContentProcess(repo.Object, ows.Object, null!);

        // Act & Assert
        Assert.DoesNotThrow(() => process.SafeDeleteFolder("/var/www/olie/dillon.txt"));
    }
    
    [Test]
    public void SafeDeleteFolder_ExitsEarly_EmptyPath()
    {
        // Arrange
        var repo = new Mock<IMyRepository>();
        var ows = new Mock<IOlieWebService>();
        var process = new DeleteOldContentProcess(repo.Object, ows.Object, null!);

        // Act
        process.SafeDeleteFolder(string.Empty);
        
        // Assert
        ows.Verify(v => v.DirectoryDelete(It.IsAny<string>()), Times.Never);
    }
    
    #endregion
}
