namespace olieblind.lib.Processes.Interfaces;

public interface ICreateSpcOutlookVideoProcess
{
    Task Run(string folderRoot, string fontName, string fontPath, string voiceName, int dayNumber, CancellationToken ct);
}
