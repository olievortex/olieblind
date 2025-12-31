namespace olieblind.lib.Processes;

public interface ICreateDroughtMonitorVideoProcess
{
    Task RunAsync(string folderRoot, string voiceName, CancellationToken ct);
}
