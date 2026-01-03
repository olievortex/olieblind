namespace olieblind.lib.Processes.Interfaces;

public interface ISpcMesosProcess
{
    Task Run(int year, bool isUpdateOnly, string goldPath, CancellationToken ct);
}
