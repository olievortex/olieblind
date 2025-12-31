namespace olieblind.lib.Maintenance;

public interface IMySqlMaintenance
{
    List<BackupFile> GetBackups();
}
