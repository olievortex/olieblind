using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;

namespace olieblind.lib.Video;

public class CommonProcess(IOlieWebService ows, IOlieImageService ois, IMyRepository repo) : ICommonProcess
{
    private const string Mp4Extension = ".mp4";

    public async Task<string> ImagesToMp4Async(List<string> files, string wavPath, string ffmpegPath, string ffmpegCodec, int imageLengthSeconds, CancellationToken ct)
    {
        var tempPath = OlieCommon.CreateLocalTmpPath(Mp4Extension);
        await ois.CreateMp4FromImagesAndAudio(files, imageLengthSeconds, wavPath, tempPath,
            ffmpegPath, ffmpegCodec, ct);

        return tempPath;
    }

    public async Task PushVideoToMySqlAsync(string video, string category, string poster, string title, string transcript, string posterPath, string videoPath, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var productVideoEntity = new ProductVideoEntity
        {
            Category = category,
            PosterUrl = poster,
            Timestamp = now,
            Title = title,
            Transcript = transcript,
            VideoUrl = video,
            PosterLocalPath = posterPath,
            VideoLocalPath = videoPath,
            IsActive = true
        };

        await repo.ProductVideoCreate(productVideoEntity, ct);
    }

    public async Task<TimeSpan> GenerateSpeechAsync(string voiceName, string path, string script, int imageLengthSeconds, IOlieSpeechService speechService, CancellationToken ct)
    {
        var length = await speechService.SpeechGenerateAsync(voiceName, script, path, ct);

        length = TimeSpan.FromSeconds(((int)(length.TotalSeconds / imageLengthSeconds) + 1) * imageLengthSeconds);

        return length;
    }

    public string GetUploadFullPath(DateTime effectiveDate, string folderRoot, string filePrefix, string extension)
    {
        if (!folderRoot.EndsWith('/')) folderRoot += '/';

        var filename = $"{filePrefix}{effectiveDate:yyMMddHHmm}{extension}";
        var folder = $"{effectiveDate:yyyy}/{effectiveDate:MM}/";
        var fullPath = $"{folderRoot}{folder}{filename}";

        return fullPath;
    }

    public string GetUploadUri(DateTime effectiveDate, string uriRoot, string filePrefix, string extension)
    {
        if (!uriRoot.EndsWith('/')) uriRoot += "/";

        var filename = $"{filePrefix}{effectiveDate:yyMMddHHmm}{extension}";
        var folder = $"{effectiveDate:yyyy}/{effectiveDate:MM}/";
        var uri = $"{uriRoot}{folder}{filename}";

        return uri;
    }

    public async Task UploadFileAsync(string localFilename, string fullPath, CancellationToken ct)
    {
        ows.FileMakeDirectory(fullPath);

        var data = await ows.FileReadAllBytes(localFilename, ct);
        await ows.FileWriteAllBytes(fullPath, data, ct);
    }

    public List<string> CreateStoryboard(List<string> sources, int imageLengthSeconds, TimeSpan duration)
    {
        var result = new List<string>();
        var sourcePtr = 0;

        for (var position = 0; position < duration.TotalSeconds; position += imageLengthSeconds)
        {
            result.Add(sources[sourcePtr++]);

            if (sourcePtr == sources.Count) sourcePtr = 0;
        }

        return result;
    }
}