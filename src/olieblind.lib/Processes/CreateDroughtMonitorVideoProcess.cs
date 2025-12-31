using olieblind.lib.DroughtMonitor;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;
using olieblind.lib.Video;
using System.Drawing;

namespace olieblind.lib.Processes;

public class CreateDroughtMonitorVideoProcess(
    IOlieWebService ows,
    IOlieImageService ois,
    IOlieConfig config,
    ICommonProcess common,
    IDroughtMonitor droughtMonitor,
    IDroughtMonitorScripting scripting,
    IOlieSpeechService speechService) : ICreateDroughtMonitorVideoProcess
{
    private const string FilePrefix = "DroughtMonitor";
    private const string Mp4Extension = ".mp4";
    private const string PngExtension = ".png";
    private const string WavExtension = ".wav";
    private const string Category = "Drought Monitor";

    public Point FinalSize { get; init; } = new(1280, 720);
    public Point PosterSize { get; init; } = new(640, 360);
    public int ImageLengthSeconds { get; init; } = 5;

    public async Task RunAsync(string folderRoot, string voiceName, CancellationToken ct)
    {
        // Acquire a product
        var xml = await droughtMonitor.GetCurrentDroughtMonitorXmlAsync(ct);
        var product = droughtMonitor.GetProductFromXml(xml);

        // Generate the script
        var script = scripting.CreateDefaultScript(product, ct);
        var transcript = scripting.CreateDefaultTranscript(product);
        var title = scripting.CreateDefaultTitle(product);

        // Text to speech
        var localWavPath = OlieCommon.CreateLocalTmpPath(WavExtension);
        var timeSpanWav = await common.GenerateSpeechAsync(voiceName, localWavPath, script, ImageLengthSeconds, speechService, ct);

        // Process images
        var (imageList, posterFilename) = await GenerateImagesAsync(ct);
        var posterUri = common.GetUploadUri(product.EffectiveDate, config.BaseVideoUrl, FilePrefix, PngExtension);
        var posterFullPath = common.GetUploadFullPath(product.EffectiveDate, folderRoot, FilePrefix, PngExtension);

        // Create the story board
        var storyboard = common.CreateStoryboard(imageList, ImageLengthSeconds, timeSpanWav);

        // Create the video
        var mp4Filename = await common.ImagesToMp4Async(storyboard, localWavPath, config.FfmpegPath, config.FfmpegCodec, ImageLengthSeconds, ct);
        var videoUri = common.GetUploadUri(product.EffectiveDate, config.BaseVideoUrl, FilePrefix, Mp4Extension);
        var videoFullPath = common.GetUploadFullPath(product.EffectiveDate, folderRoot, FilePrefix, Mp4Extension);

        // Upload final video and poster
        await common.UploadFileAsync(mp4Filename, videoFullPath, ct);
        await common.UploadFileAsync(posterFilename, posterFullPath, ct);

        // Add record to the database
        await common.PushVideoToMySqlAsync(videoUri, Category, posterUri, title, transcript, posterFullPath, videoFullPath, ct);

        // Cleanup
        OlieCommon.DeleteTempFiles(ows, imageList, localWavPath, mp4Filename);
    }

    #region Private Helpers

    private async Task<(List<string>, string)> GenerateImagesAsync(CancellationToken ct)
    {
        var imageNames = droughtMonitor.GetImageNames();
        var result = new List<string>();
        var first = true;
        var posterFile = string.Empty;

        foreach (var imageName in imageNames)
        {
            var source = await ows.ApiGetBytes(imageName, ct);
            var fullSizeFile = await GetBrandedImage(source, imageName, FinalSize, ct);
            result.Add(fullSizeFile);

            if (!first) continue;

            posterFile = await GetBrandedImage(source, imageName, PosterSize, ct);
            first = false;
        }

        return (result, posterFile);
    }

    private async Task<string> GetBrandedImage(byte[] bitmap, string imageName, Point finalSize, CancellationToken ct)
    {
        var bytes = await ois.Resize(bitmap, finalSize, ct);
        var localFile = OlieCommon.CreateLocalTmpPath(Path.GetExtension(imageName));

        await ows.FileWriteAllBytes(localFile, bytes, ct);

        return localFile;
    }

    #endregion
}