namespace olieblind.lib.Processes.Interfaces;

public interface ICreateDroughtMonitorVideoProcess
{
    Task Run(string folderRoot, string voiceName, CancellationToken ct);
}
