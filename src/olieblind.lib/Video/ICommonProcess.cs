using olieblind.lib.Services.Speech;

namespace olieblind.lib.Video;

public interface ICommonProcess
{
    List<string> CreateStoryboard(List<string> sources, int imageLengthSeconds, TimeSpan duration);
    Task<TimeSpan> GenerateSpeechAsync(string voiceName, string path, string script, int imageLengthSeconds, IOlieSpeechService speechService, CancellationToken ct);
    string GetUploadFullPath(DateTime effectiveDate, string folderRoot, string filePrefix, string extension);
    string GetUploadUri(DateTime effectiveDate, string uriRoot, string filePrefix, string extension);
    Task<string> ImagesToMp4Async(List<string> files, string wavPath, string ffmpegPath, string ffmpegCodec, int imageLengthSeconds, CancellationToken ct);
    Task PushVideoToMySqlAsync(string video, string category, string poster, string title, string transcript, string posterPath, string videoPath, CancellationToken ct);
    Task UploadFileAsync(string localFilename, string fullPath, CancellationToken ct);
}
