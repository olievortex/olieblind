using Azure.Storage.Blobs;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Maintenance;
using olieblind.lib.Services;

namespace olieblind.lib.Processes;

public class DeleteOldContentProcess(IMyRepository repo, IOlieWebService ows, IMySqlMaintenance mysql) : IDeleteOldContentProcess
{
    const string DroughtMonitor = "Drought Monitor";

    public async Task Run(BlobContainerClient bcc, CancellationToken ct)
    {
        var videos = await repo.ProductVideoGetList(ct);

        // Delete SPC Outlooks after 2 weeks
        await DeleteSpcOutlookVideos(videos, ct);

        // Delete Drought Monitor videos after 60 days
        await DeleteDroughtMonitorVideos(videos, ct);

        var maps = await repo.ProductMapList(ct);

        // Delete Maps after 7 days
        await DeleteModelForecastMaps(maps, ct);

        // Compress and upload MySQL Backups, delete after 7 days
        var backups = mysql.GetBackups();
        await CompressBackups(bcc, backups, ct);
    }

    private async Task CompressBackups(BlobContainerClient bcc, List<BackupFile> files, CancellationToken ct)
    {
        foreach (var file in files)
        {
            if (!file.IsCompressed)
            {
                var destination = Path.ChangeExtension(file.BackupFilePath, ".sql.gz");
                ows.CompressGzip(file.BackupFilePath, destination);
                ows.FileDelete(file.BackupFilePath);

                file.BackupFilePath = destination;
                await ows.BlobUploadFile(bcc, Path.GetFileName(destination), destination, ct);
            }

            if (file.Effective < DateTime.UtcNow.AddDays(-7))
            {
                SafeDeleteFile(file.BackupFilePath);
            }
        }
    }

    private async Task DeleteModelForecastMaps(List<ProductMapEntity> maps, CancellationToken ct)
    {
        var query = maps
            .Where(w => w.Timestamp < DateTime.UtcNow.AddDays(-7))
            .ToList();
        
        foreach (var item in query)
        {
            var items = await repo.ProductMapItemList(item.Id, ct);
            var folderPath = string.Empty;
            
            foreach (var mapItem in items)
            {
                folderPath = mapItem.LocalPath;
                SafeDeleteFile(mapItem.LocalPath);
                mapItem.IsActive = false;

                await repo.ProductMapItemUpdate(mapItem, ct);
            }

            item.IsActive = false;
            await repo.ProductMapUpdate(item, ct);

            SafeDeleteFolder(folderPath);
        }
    }

    private async Task DeleteDroughtMonitorVideos(List<ProductVideoEntity> videos, CancellationToken ct)
    {
        var query = videos
            .Where(w => w.Category == DroughtMonitor && w.Timestamp < DateTime.UtcNow.AddDays(-60))
            .ToList();

        await DeleteVideos(query, ct);
    }

    private async Task DeleteSpcOutlookVideos(List<ProductVideoEntity> videos, CancellationToken ct)
    {
        var query = videos
            .Where(w => w.Category != DroughtMonitor && w.Timestamp < DateTime.UtcNow.AddDays(-14))
            .ToList();

        await DeleteVideos(query, ct);
    }

    private async Task DeleteVideos(List<ProductVideoEntity> videos, CancellationToken ct)
    {
        foreach (var item in videos)
        {
            SafeDeleteFile(item.PosterLocalPath);
            SafeDeleteFile(item.VideoLocalPath);

            item.IsActive = false;
            await repo.ProductVideoUpdate(item, ct);
        }
    }

    public void SafeDeleteFolder(string path)
    {
        try
        {
            var folderPath = Path.GetDirectoryName(path.Replace('\\', '/'));
            if (string.IsNullOrWhiteSpace(folderPath)) return;
            ows.DirectoryDelete(folderPath);
        }
        catch
        {
            // Ignore
        }
    }
    
    public void SafeDeleteFile(string path)
    {
        try
        {
            ows.FileDelete(path);
        }
        catch
        {
            // Ignore
        }
    }
}