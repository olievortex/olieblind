namespace olieblind.lib.Processes;

public interface ICreateSpcOutlookVideoProcess
{
    Task RunAsync(string folderRoot, string fontName, string fontPath, string voiceName, int dayNumber, CancellationToken ct);
}
