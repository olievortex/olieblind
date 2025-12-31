using Moq;
using olieblind.lib.Maintenance;
using olieblind.lib.Services;

namespace olieblind.test.MaintenanceTests;

public class MySqlMaintenanceTests
{
    [Test]
    public void GetBackups_ShouldReturnOrderedBackupFiles()
    {
        // Arrange
        var mockOlieWebService = new Mock<IOlieWebService>();
        var mockOlieConfig = new Mock<IOlieConfig>();
        mockOlieConfig.Setup(c => c.MySqlBackupPath).Returns("/backups");
        var backupFiles = new List<string>
        {
            "/var/backups/mysql/20230101_010000_olieblind_dev.sql",
            "/var/backups/mysql/20230201_010000_olieblind.sql",
            "/var/backups/mysql/20230301_010000_olieblind_dev.sql",
            "/var/backups/mysql/20230115_010000_olieblind_dev.sql",
            "/var/backups/mysql/otherfile.txt"
        };
        mockOlieWebService.Setup(s => s.DirectoryList("/backups")).Returns(backupFiles);
        var mySqlMaintenance = new MySqlMaintenance(mockOlieWebService.Object, mockOlieConfig.Object);

        // Act
        var result = mySqlMaintenance.GetBackups();

        // Assert
        Assert.That(result, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].BackupFilePath, Is.EqualTo("/var/backups/mysql/20230301_010000_olieblind_dev.sql"));
            Assert.That(result[1].BackupFilePath, Is.EqualTo("/var/backups/mysql/20230201_010000_olieblind.sql"));
            Assert.That(result[2].BackupFilePath, Is.EqualTo("/var/backups/mysql/20230115_010000_olieblind_dev.sql"));
            Assert.That(result[3].BackupFilePath, Is.EqualTo("/var/backups/mysql/20230101_010000_olieblind_dev.sql"));
        }
    }
}
