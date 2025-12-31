using olieblind.lib.Services;

namespace olieblind.lib.Maintenance;

public class MySqlMaintenance(IOlieWebService ows, IOlieConfig config) : IMySqlMaintenance
{
    public List<BackupFile> GetBackups()
    {
        var files = ows.DirectoryList(config.MySqlBackupPath);
        var result = new List<BackupFile>();

        foreach (var file in files)
        {
            if (!(file.Contains("_olieblind_dev.sql") || file.Contains("_olieblind.sql"))) continue;

            result.Add(new BackupFile { BackupFilePath = file });
        }

        return [.. result.OrderByDescending(o => o.Effective)];
    }
}
