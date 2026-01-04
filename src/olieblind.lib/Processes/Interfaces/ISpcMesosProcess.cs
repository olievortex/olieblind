namespace olieblind.lib.Processes.Interfaces;

public interface ISpcMesosProcess
{
    Task Run(int year, string goldPath, CancellationToken ct);
}
